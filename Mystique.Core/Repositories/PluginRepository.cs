using Dapper;
using Microsoft.EntityFrameworkCore;
using Mystique.Core.DTOs;
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

        public async Task AddPluginAsync(AddPluginDTO dto)
        {
            var plugin = new PluginViewModel
            {
                PluginId = dto.PluginId,
                Name = dto.Name,
                UniqueKey = dto.UniqueKey,
                Version = dto.Version,
                DisplayName = dto.DisplayName,
                IsEnable = false,
            };
            pluginDbContext.Plugins.Add(plugin);
            await unitOfWork.SaveAsync();
        }

        public async Task UpdatePluginVersionAsync(Guid pluginId, string version)
        {
            var find = await pluginDbContext.Plugins.FirstOrDefaultAsync(o => o.PluginId == pluginId);
            if (find == null)
            {
                return;
            }
            find.Version = version;
            await unitOfWork.SaveAsync();
        }

        public async Task<List<PluginListItemViewModel>> GetAllPluginsAsync()
        {
            var plugins = await pluginDbContext.Plugins.AsNoTracking().ToListAsync();
            return plugins.Select(o => new PluginListItemViewModel
            {
                PluginId = o.PluginId,
                Name = o.Name,
                UniqueKey = o.UniqueKey,
                Version = o.Version,
                DisplayName = o.DisplayName,
                IsEnable = o.IsEnable,
            }).ToList();
        }

        public async Task<List<PluginListItemViewModel>> GetAllEnabledPluginsAsync()
        {
            var plugins = await GetAllPluginsAsync();
            return plugins.Where(o => o.IsEnable).ToList();
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

        public async Task RunDownMigrationsAsync(Guid pluginId)
        {
            var plugin = await pluginDbContext.Plugins.FirstOrDefaultAsync(o => o.PluginId == pluginId);
            if (plugin == null)
            {
                return;
            }
            var downs = await pluginDbContext.PluginMigrations.Where(o => o.Plugin == plugin).OrderByDescending(o => o.Version).Select(o => o.Down).ToListAsync();
            var conn = pluginDbContext.Database.GetDbConnection();
            foreach (var down in downs)
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                await conn.ExecuteAsync(down, new { pluginId, });
            }
        }
    }
}
