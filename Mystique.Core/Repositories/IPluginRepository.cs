using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystique.Core.Repositories
{
    public interface IPluginRepository
    {
        Task<List<PluginViewModel>> GetAllPluginsAsync();

        Task<List<PluginViewModel>> GetAllEnabledPluginsAsync();

        Task AddPluginAsync(PluginViewModel dto);

        Task<PluginViewModel> GetPluginAsync(Guid pluginId);

        Task<PluginViewModel> GetPluginAsync(string pluginName);

        Task SetPluginStatusAsync(Guid pluginId, bool enable);

        Task DeletePluginAsync(Guid pluginId);
    }
}
