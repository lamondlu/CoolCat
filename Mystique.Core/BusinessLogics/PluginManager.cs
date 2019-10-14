using Microsoft.Extensions.Options;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Models;
using Mystique.Core.Repositories;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Version = Mystique.Core.DomainModel.Version;

namespace Mystique.Core.BusinessLogics
{
    public class PluginManager : IPluginManager
    {
        private readonly IUnitOfWork unitOfWork = null;
        private readonly string connectionString = null;
        private readonly IMvcModuleSetup mvcModuleSetup = null;

        public PluginManager(IUnitOfWork unitOfWork, IOptions<ConnectionStringSetting> connectionStringSettingAccessor, IMvcModuleSetup mvcModuleSetup)
        {
            this.unitOfWork = unitOfWork;
            connectionString = connectionStringSettingAccessor.Value.ConnectionString;
            this.mvcModuleSetup = mvcModuleSetup;
        }

        public List<PluginListItemViewModel> GetAllPlugins() => unitOfWork.PluginRepository.GetAllPlugins();

        public List<PluginListItemViewModel> GetAllEnabledPlugins() => unitOfWork.PluginRepository.GetAllEnabledPlugins();

        public PluginViewModel GetPlugin(Guid pluginId) => unitOfWork.PluginRepository.GetPlugin(pluginId);

        public void EnablePlugin(Guid pluginId)
        {
            var module = unitOfWork.PluginRepository.GetPlugin(pluginId);
            unitOfWork.PluginRepository.SetPluginStatus(pluginId, true);

            mvcModuleSetup.EnableModule(module.Name);
        }

        public void DeletePlugin(Guid pluginId)
        {
            var plugin = unitOfWork.PluginRepository.GetPlugin(pluginId);

            if (plugin.IsEnable)
            {
                DisablePlugin(pluginId);
            }

            unitOfWork.PluginRepository.RunDownMigrations(pluginId);
            unitOfWork.PluginRepository.DeletePlugin(pluginId);
            unitOfWork.Commit();

            mvcModuleSetup.DeleteModule(plugin.Name);
        }

        public void DisablePlugin(Guid pluginId)
        {
            var module = unitOfWork.PluginRepository.GetPlugin(pluginId);
            unitOfWork.PluginRepository.SetPluginStatus(pluginId, false);
            mvcModuleSetup.DisableModule(module.Name);
        }

        public void AddPlugins(PluginPackage pluginPackage)
        {
            var existedPlugin = unitOfWork.PluginRepository.GetPlugin(pluginPackage.Configuration.Name);

            if (existedPlugin == null)
            {
                InitializePlugin(pluginPackage);
            }
            else if (new Version(pluginPackage.Configuration.Version) > new Version(existedPlugin.Version))
            {
                UpgradePlugin(pluginPackage, existedPlugin);
            }
            else if (new Version(pluginPackage.Configuration.Version) == new Version(existedPlugin.Version))
            {
                throw new Exception("The package version is same as the current plugin version.");
            }
            else
            {
                DegradePlugin(pluginPackage, existedPlugin);
            }
        }

        private void InitializePlugin(PluginPackage pluginPackage)
        {
            var plugin = new DTOs.AddPluginDTO
            {
                Name = pluginPackage.Configuration.Name,
                DisplayName = pluginPackage.Configuration.DisplayName,
                PluginId = Guid.NewGuid(),
                UniqueKey = pluginPackage.Configuration.UniqueKey,
                Version = pluginPackage.Configuration.Version
            };

            unitOfWork.PluginRepository.AddPlugin(plugin);
            unitOfWork.Commit();

            var versions = pluginPackage.GetAllMigrations(connectionString);

            foreach (var version in versions)
            {
                version.MigrationUp(plugin.PluginId);
            }

            pluginPackage.SetupFolder();
        }

        public void UpgradePlugin(PluginPackage pluginPackage, PluginViewModel oldPlugin)
        {
            unitOfWork.PluginRepository.UpdatePluginVersion(oldPlugin.PluginId, pluginPackage.Configuration.Version);
            unitOfWork.Commit();

            var migrations = pluginPackage.GetAllMigrations(connectionString);

            var pendingMigrations = migrations.Where(p => p.Version > oldPlugin.Version);

            foreach (var migration in pendingMigrations)
            {
                migration.MigrationUp(oldPlugin.PluginId);
            }

            pluginPackage.SetupFolder();
        }

        public void DegradePlugin(PluginPackage pluginPackage, PluginViewModel oldPlugin)
        {
            unitOfWork.PluginRepository.UpdatePluginVersion(oldPlugin.PluginId, pluginPackage.Configuration.Version);
            unitOfWork.Commit();
        }
    }
}
