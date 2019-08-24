using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Models;
using Mystique.Core.Repositories;
using Mystique.Core.ViewModels;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;
using System.Text;
using System.Linq;

namespace Mystique.Core.BusinessLogics
{
    public class PluginManager : IPluginManager
    {
        private IUnitOfWork _unitOfWork = null;
        private string _connectionString = null;

        public PluginManager(IUnitOfWork unitOfWork, IOptions<ConnectionStringSetting> connectionStringSettingAccessor)
        {
            _unitOfWork = unitOfWork;
            _connectionString = connectionStringSettingAccessor.Value.ConnectionString;
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
            _unitOfWork.PluginRepository.SetPluginStatus(pluginId, true);
        }

        public void DeletePlugin(Guid pluginId)
        {
            var plugin = _unitOfWork.PluginRepository.GetPlugin(pluginId);
            _unitOfWork.PluginRepository.RunDownMigrations(pluginId);
            _unitOfWork.PluginRepository.DeletePlugin(pluginId);

            _unitOfWork.Commit();
        }

        public void DisablePlugin(Guid pluginId)
        {
            _unitOfWork.PluginRepository.SetPluginStatus(pluginId, false);
        }

        public void AddPlugins(PluginPackage pluginPackage)
        {
            var existedPlugin = _unitOfWork.PluginRepository.GetPlugin(pluginPackage.Configuration.Name);

            if (existedPlugin == null)
            {
                InitializePlugin(pluginPackage);
            }
            else if (new DomainModel.Version(pluginPackage.Configuration.Version) > new DomainModel.Version(existedPlugin.Version))
            {
                UpgradePlugin(pluginPackage, existedPlugin);
            }
            else if (new DomainModel.Version(pluginPackage.Configuration.Version) == new DomainModel.Version(existedPlugin.Version))
            {
                throw new Exception("The package version is same as the current plugin version.");
            }
            else
            {
                DegradePlugin(pluginPackage);
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

            _unitOfWork.PluginRepository.AddPlugin(plugin);
            _unitOfWork.Commit();

            var versions = pluginPackage.GetAllMigrations(_connectionString);

            foreach (var version in versions)
            {
                version.MigrationUp(plugin.PluginId);
            }

            pluginPackage.SetupFolder();
        }

        public void UpgradePlugin(PluginPackage pluginPackage, PluginViewModel oldPlugin)
        {
            _unitOfWork.PluginRepository.UpdatePluginVersion(oldPlugin.PluginId, pluginPackage.Configuration.Version);
            _unitOfWork.Commit();

            var migrations = pluginPackage.GetAllMigrations(_connectionString);

            var pendingMigrations = migrations.Where(p => p.Version > oldPlugin.Version);

            foreach (var migration in pendingMigrations)
            {
                migration.MigrationUp(oldPlugin.PluginId);
            }

            pluginPackage.SetupFolder();
        }

        public void DegradePlugin(PluginPackage pluginPackage)
        {
            throw new NotImplementedException();
        }
    }
}
