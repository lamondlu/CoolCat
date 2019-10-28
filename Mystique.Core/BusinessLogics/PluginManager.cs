using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Repositories;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Version = Mystique.Core.DomainModel.Version;

namespace Mystique.Core.BusinessLogics
{
    public class PluginManager : IPluginManager
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPluginRepository pluginRepository;
        private readonly IMvcModuleSetup mvcModuleSetup;

        public PluginManager(IUnitOfWork unitOfWork, IPluginRepository pluginRepository, IMvcModuleSetup mvcModuleSetup)
        {
            this.unitOfWork = unitOfWork;
            this.pluginRepository = pluginRepository;
            this.mvcModuleSetup = mvcModuleSetup;
        }

        public async Task<List<PluginListItemViewModel>> GetAllPluginsAsync() => await pluginRepository.GetAllPluginsAsync();

        public async Task<List<PluginListItemViewModel>> GetAllEnabledPluginsAsync() => await pluginRepository.GetAllEnabledPluginsAsync();

        public async Task<PluginViewModel> GetPluginAsync(Guid pluginId) => await pluginRepository.GetPluginAsync(pluginId);

        public async Task EnablePluginAsync(Guid pluginId)
        {
            var module = await pluginRepository.GetPluginAsync(pluginId);
            await pluginRepository.SetPluginStatusAsync(pluginId, true);

            await mvcModuleSetup.EnableModuleAsync(module.Name);
        }

        public async Task DeletePluginAsync(Guid pluginId)
        {
            var plugin = await pluginRepository.GetPluginAsync(pluginId);

            if (plugin.IsEnable)
            {
                await DisablePluginAsync(pluginId);
            }

            await pluginRepository.RunDownMigrationsAsync(pluginId);
            await pluginRepository.DeletePluginAsync(pluginId);
            await unitOfWork.SaveAsync();

            await mvcModuleSetup.DeleteModuleAsync(plugin.Name);
        }

        public async Task DisablePluginAsync(Guid pluginId)
        {
            var module = await pluginRepository.GetPluginAsync(pluginId);
            await pluginRepository.SetPluginStatusAsync(pluginId, false);
            await mvcModuleSetup.DisableModuleAsync(module.Name);
        }

        public async Task AddPluginsAsync(PluginPackage pluginPackage)
        {
            var existedPlugin = await pluginRepository.GetPluginAsync(pluginPackage.PluginConfiguration.Name);
            if (existedPlugin != null)
            {
                await DeletePluginAsync(existedPlugin.PluginId);
            }
            var pluginId = await InitializePluginAsync(pluginPackage);

            if (existedPlugin != null && new Version(pluginPackage.PluginConfiguration.Version) > new Version(existedPlugin.Version))
            {
                await UpgradePluginAsync(pluginPackage, existedPlugin);
            }
            else if (existedPlugin != null && new Version(pluginPackage.PluginConfiguration.Version) < new Version(existedPlugin.Version))
            {
                await DegradePluginAsync(pluginPackage, existedPlugin);
            }
            else
            {
                // do nothing
            }

            pluginPackage.SetupFolder();

            if (existedPlugin?.IsEnable == true)
            {
                await EnablePluginAsync(pluginId);
            }
        }

        private async Task<Guid> InitializePluginAsync(PluginPackage pluginPackage)
        {
            var plugin = new DTOs.AddPluginDTO
            {
                Name = pluginPackage.PluginConfiguration.Name,
                DisplayName = pluginPackage.PluginConfiguration.DisplayName,
                PluginId = Guid.NewGuid(),
                UniqueKey = pluginPackage.PluginConfiguration.UniqueKey,
                Version = pluginPackage.PluginConfiguration.Version
            };
            await pluginRepository.AddPluginAsync(plugin);
            await unitOfWork.SaveAsync();

            foreach (var version in pluginPackage.GetAllMigrations())
            {
                await version.MigrationUpAsync(plugin.PluginId);
            }

            return plugin.PluginId;
        }

        private async Task UpgradePluginAsync(PluginPackage pluginPackage, PluginViewModel oldPlugin)
        {
            await pluginRepository.UpdatePluginVersionAsync(oldPlugin.PluginId, pluginPackage.PluginConfiguration.Version);
            await unitOfWork.SaveAsync();

            foreach (var migration in pluginPackage.GetAllMigrations())
            {
                if (migration.Version > oldPlugin.Version)
                {
                    await migration.MigrationUpAsync(oldPlugin.PluginId);
                }
            }
        }

        private async Task DegradePluginAsync(PluginPackage pluginPackage, PluginViewModel oldPlugin)
        {
            await pluginRepository.UpdatePluginVersionAsync(oldPlugin.PluginId, pluginPackage.PluginConfiguration.Version);
            await unitOfWork.SaveAsync();
        }
    }
}
