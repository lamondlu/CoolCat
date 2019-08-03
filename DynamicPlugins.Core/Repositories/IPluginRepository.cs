using DynamicPlugins.Core.DTOs;
using DynamicPlugins.Core.Helpers;
using DynamicPlugins.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Repositories
{
    public interface IPluginRepository
    {
        List<PluginListItemViewModel> GetAllPlugins();

        List<PluginListItemViewModel> GetAllEnabledPlugins();

        void AddPlugin(AddPluginDTO dto);

        PluginViewModel GetPlugin(Guid pluginId);

        void SetPluginStatus(Guid pluginId, bool enable);
    }
}
