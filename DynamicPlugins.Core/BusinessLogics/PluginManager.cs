using DynamicPlugins.Core.Contracts;
using DynamicPlugins.Core.DomainModel;
using DynamicPlugins.Core.Models;
using DynamicPlugins.Core.Repositories;
using DynamicPlugins.Core.ViewModels;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.BusinessLogics
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

        public void AddPlugins(PluginPackage pluginPackage)
        {
            try
            {
                var versions = pluginPackage.GetAllMigrations(_connectionString);

                foreach (var version in versions)
                {
                    version.Up();
                }

                _unitOfWork.PluginRepository.AddPlugin(new DTOs.AddPluginDTO
                {
                    Name = pluginPackage.Configuration.Name,
                    DisplayName = pluginPackage.Configuration.DisplayName,
                    PluginId = Guid.NewGuid(),
                    UniqueKey = pluginPackage.Configuration.UniqueKey,
                    Version = pluginPackage.Configuration.Version
                });

                _unitOfWork.Commit();
            }
            catch
            {

            }
            finally
            {

            }
        }
    }
}
