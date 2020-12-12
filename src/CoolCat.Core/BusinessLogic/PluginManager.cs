using Microsoft.Extensions.Options;
using CoolCat.Core.Contracts;
using CoolCat.Core.DomainModel;
using CoolCat.Core.Models;
using CoolCat.Core.Repositories;
using CoolCat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Version = CoolCat.Core.DomainModel.Version;

namespace CoolCat.Core.BusinessLogic
{
    public class PluginManager : IPluginManager
    {
        private readonly IUnitOfWork _unitOfWork = null;
        private readonly string _connectionString = null;
        private readonly IMvcModuleSetup _mvcModuleSetup = null;

        public PluginManager(IUnitOfWork unitOfWork, IOptions<ConnectionStringSetting> connectionStringSettingAccessor, IMvcModuleSetup mvcModuleSetup)
        {
            _unitOfWork = unitOfWork;
            _connectionString = connectionStringSettingAccessor.Value.ConnectionString;
            _mvcModuleSetup = mvcModuleSetup;
        }

        public List<PluginListItemViewModel> GetAllPlugins()
        {
            return _unitOfWork.PluginRepository.GetAllPlugins();
        }

        public List<PluginListItemViewModel> GetAllEnabledPlugins()
        {
            return _unitOfWork.PluginRepository.GetAllEnabledPlugins();
        }

        public PluginViewModel GetPlugin(Guid pluginId)
        {
            return _unitOfWork.PluginRepository.GetPlugin(pluginId);
        }

        public void EnablePlugin(Guid pluginId)
        {
            PluginViewModel module = _unitOfWork.PluginRepository.GetPlugin(pluginId);
            _unitOfWork.PluginRepository.SetPluginStatus(pluginId, true);

            _mvcModuleSetup.EnableModule(module.Name);
        }

        public void DeletePlugin(Guid pluginId)
        {
            PluginViewModel plugin = _unitOfWork.PluginRepository.GetPlugin(pluginId);

            if (plugin.IsEnable)
            {
                DisablePlugin(pluginId);
            }

            _unitOfWork.PluginRepository.RunDownMigrations(pluginId);
            _unitOfWork.PluginRepository.DeletePlugin(pluginId);
            _unitOfWork.Commit();

            _mvcModuleSetup.DeleteModule(plugin.Name);
        }

        public void DisablePlugin(Guid pluginId)
        {
            PluginViewModel module = _unitOfWork.PluginRepository.GetPlugin(pluginId);
            _unitOfWork.PluginRepository.SetPluginStatus(pluginId, false);
            _mvcModuleSetup.DisableModule(module.Name);
        }

        public void AddPlugins(PluginPackage pluginPackage)
        {
            PluginViewModel existedPlugin = _unitOfWork.PluginRepository.GetPlugin(pluginPackage.Configuration.Name);

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
            DTOs.AddPluginDTO plugin = new DTOs.AddPluginDTO
            {
                Name = pluginPackage.Configuration.Name,
                DisplayName = pluginPackage.Configuration.DisplayName,
                UniqueKey = pluginPackage.Configuration.UniqueKey,
                Version = pluginPackage.Configuration.Version
            };

            _unitOfWork.PluginRepository.AddPlugin(plugin);
            _unitOfWork.Commit();

            List<IMigration> versions = pluginPackage.GetAllMigrations(_connectionString);

            foreach (IMigration version in versions)
            {
                version.MigrateUp(plugin.PluginId);
            }

            pluginPackage.SetupFolder();
        }

        public void UpgradePlugin(PluginPackage pluginPackage, PluginViewModel oldPlugin)
        {
            _unitOfWork.PluginRepository.UpdatePluginVersion(oldPlugin.PluginId, pluginPackage.Configuration.Version);
            _unitOfWork.Commit();

            List<IMigration> migrations = pluginPackage.GetAllMigrations(_connectionString);

            IEnumerable<IMigration> pendingMigrations = migrations.Where(p => p.Version > oldPlugin.Version);

            foreach (IMigration migration in pendingMigrations)
            {
                migration.MigrateUp(oldPlugin.PluginId);
            }

            pluginPackage.SetupFolder();
        }

        public void DegradePlugin(PluginPackage pluginPackage, PluginViewModel oldPlugin)
        {
            _unitOfWork.PluginRepository.UpdatePluginVersion(oldPlugin.PluginId, pluginPackage.Configuration.Version);
            _unitOfWork.Commit();
        }

        public List<CollectibleAssemblyLoadContext> GetAllContexts()
        {
            return PluginsLoadContexts.All();
        }
    }
}
