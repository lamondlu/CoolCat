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

        void AddPlugin(AddPluginDTO dto);
    }
}
