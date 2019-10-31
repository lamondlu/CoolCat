using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Repositories;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<List<PluginViewModel>> GetAllPluginsAsync() => await pluginRepository.GetAllPluginsAsync();

        public async Task<List<PluginViewModel>> GetAllEnabledPluginsAsync() => await pluginRepository.GetAllEnabledPluginsAsync();

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
            if (plugin?.IsEnable == true)
            {
                await DisablePluginAsync(pluginId);
            }

            await pluginRepository.DeletePluginAsync(pluginId);
            await unitOfWork.SaveAsync();

            if (plugin != null)
            {
                await mvcModuleSetup.DeleteModuleAsync(plugin.Name);
            }
        }

        public async Task DisablePluginAsync(Guid pluginId)
        {
            var module = await pluginRepository.GetPluginAsync(pluginId);
            await pluginRepository.SetPluginStatusAsync(pluginId, false);
            await mvcModuleSetup.DisableModuleAsync(module.Name);
        }

        public async Task AddPluginsAsync(PluginPackage pluginPackage, bool autoEnabled = false)
        {
            var existedPlugin = await pluginRepository.GetPluginAsync(pluginPackage.PluginConfiguration.Name);
            if (existedPlugin != null)
            {
                await DeletePluginAsync(existedPlugin.PluginId);
            }
            var pluginId = await InitializePluginAsync(pluginPackage);
            if (existedPlugin?.IsEnable == true || autoEnabled)
            {
                await EnablePluginAsync(pluginId);
            }
        }

        private async Task<Guid> InitializePluginAsync(PluginPackage pluginPackage)
        {
            var plugin = new PluginViewModel
            {
                Name = pluginPackage.PluginConfiguration.Name,
                DisplayName = pluginPackage.PluginConfiguration.DisplayName,
                PluginId = Guid.NewGuid(),
                UniqueKey = pluginPackage.PluginConfiguration.UniqueKey,
                Version = pluginPackage.PluginConfiguration.Version
            };
            await pluginRepository.AddPluginAsync(plugin);
            await unitOfWork.SaveAsync();

            pluginPackage.SetupFolder();
            return plugin.PluginId;
        }
    }
}
