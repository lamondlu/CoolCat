using Microsoft.EntityFrameworkCore;
using Mystique.Core.Interfaces;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Mystique.Core.Repositories
{
    public class PluginRepository : IPluginRepository
    {
        private readonly PluginDbContext pluginDbContext;
        private readonly IUnitOfWork unitOfWork;

        public PluginRepository(PluginDbContext pluginDbContext, IUnitOfWork unitOfWork)
        {
            this.pluginDbContext = pluginDbContext;
            this.unitOfWork = unitOfWork;
        }

        public async Task<List<PluginViewModel>> GetAllPluginsAsync()
        {
            return await pluginDbContext.Plugins.AsNoTracking().ToListAsync();
        }

        public async Task<List<PluginViewModel>> GetAllEnabledPluginsAsync()
        {
            return await pluginDbContext.Plugins.AsNoTracking().Where(o => o.IsEnable).ToListAsync();
        }

        public async Task AddPluginAsync(PluginViewModel plugin)
        {
            pluginDbContext.Plugins.Add(plugin);
            await unitOfWork.SaveAsync();
        }

        public async Task SetPluginStatusAsync(Guid pluginId, bool enable)
        {
            var plugin = await pluginDbContext.Plugins.FirstOrDefaultAsync(o => o.PluginId == pluginId);
            if (plugin == null)
            {
                return;
            }
            plugin.IsEnable = enable;
            await unitOfWork.SaveAsync();
        }

        public async Task<PluginViewModel> GetPluginAsync(string pluginName)
        {
            return await pluginDbContext.Plugins.AsNoTracking().FirstOrDefaultAsync(o => o.Name == pluginName);
        }

        public async Task<PluginViewModel> GetPluginAsync(Guid pluginId)
        {
            return await pluginDbContext.Plugins.AsNoTracking().FirstOrDefaultAsync(o => o.PluginId == pluginId);
        }

        public async Task DeletePluginAsync(Guid pluginId)
        {
            var plugin = await pluginDbContext.Plugins.FirstOrDefaultAsync(o => o.PluginId == pluginId);
            if (plugin == null)
            {
                return;
            }
            pluginDbContext.Plugins.Remove(plugin);
            await unitOfWork.SaveAsync();
        }

    }
}
