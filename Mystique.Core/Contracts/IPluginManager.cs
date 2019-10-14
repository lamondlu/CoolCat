using Mystique.Core.DomainModel;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystique.Core.Contracts
{
    public interface IPluginManager
    {
        Task<List<PluginListItemViewModel>> GetAllPluginsAsync();
        Task AddPluginsAsync(PluginPackage pluginPackage);
        Task<PluginViewModel> GetPluginAsync(Guid pluginId);
        Task DeletePluginAsync(Guid pluginId);
        Task EnablePluginAsync(Guid pluginId);
        Task DisablePluginAsync(Guid pluginId);
    }
}
