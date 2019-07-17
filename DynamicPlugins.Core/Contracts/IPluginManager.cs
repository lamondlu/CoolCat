using DynamicPlugins.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Contracts
{
    public interface IPluginManager
    {
        List<PluginListItemViewModel> GetAllPlugins();
    }
}
