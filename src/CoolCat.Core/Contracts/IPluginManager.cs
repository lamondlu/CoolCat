using CoolCat.Core.DomainModel;
using CoolCat.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace CoolCat.Core.Contracts
{
    public interface IPluginManager
    {
        List<PluginListItemViewModel> GetAllPlugins();

        void AddPlugins(PluginPackage pluginPackage);

        PluginViewModel GetPlugin(Guid pluginId);

        void DeletePlugin(Guid pluginId);

        void EnablePlugin(Guid pluginId);

        void DisablePlugin(Guid pluginId);

        List<CollectibleAssemblyLoadContext> GetAllContexts();
    }
}
