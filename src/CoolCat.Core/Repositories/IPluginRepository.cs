using CoolCat.Core.DTOs;
using CoolCat.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace CoolCat.Core.Repositories
{
    public interface IPluginRepository
    {
        List<PluginListItemViewModel> GetAllPlugins();

        List<PluginListItemViewModel> GetAllEnabledPlugins();

        void AddPlugin(AddPluginDTO dto);

        void UpdatePluginVersion(Guid pluginId, string version);

        PluginViewModel GetPlugin(Guid pluginId);

        PluginViewModel GetPlugin(string pluginName);

        void SetPluginStatus(Guid pluginId, bool enable);

        void DeletePlugin(Guid pluginId);

        void RunDownMigrations(Guid pluginId);
    }
}
