using Mystique.Core.DTOs;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystique.Core.Repositories
{
    public interface IPluginRepository
    {
        Task<List<PluginListItemViewModel>> GetAllPluginsAsync();

        Task<List<PluginListItemViewModel>> GetAllEnabledPluginsAsync();

        Task AddPluginAsync(AddPluginDTO dto);

        Task<PluginViewModel> GetPluginAsync(Guid pluginId);

        Task<PluginViewModel> GetPluginAsync(string pluginName);

        Task SetPluginStatusAsync(Guid pluginId, bool enable);

        Task DeletePluginAsync(Guid pluginId);
    }
}
