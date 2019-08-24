using Mystique.Core.DTOs;
using Mystique.Core.Helpers;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Repositories
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
