using Mystique.Core.DomainModel;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Contracts
{
    public interface IPluginManager
    {
        List<PluginListItemViewModel> GetAllPlugins();

        void AddPlugins(PluginPackage pluginPackage);

        PluginViewModel GetPlugin(Guid pluginId);

        void DeletePlugin(Guid pluginId);

        void EnablePlugin(Guid pluginId);

        void DisablePlugin(Guid pluginId);
    }
}
