using Dapper;
using Microsoft.EntityFrameworkCore;
using Mystique.Core.Contracts;
using Mystique.Core.Repositories;
using Mystique.Core.ViewModels;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Mystique.Core.DomainModel
{
    public abstract class BaseMigration : IMigration
    {
        private readonly PluginDbContext pluginDbContext;
        private readonly IUnitOfWork unitOfWork;

        public BaseMigration(PluginDbContext pluginDbContext, IUnitOfWork unitOfWork, Version version)
        {
            this.pluginDbContext = pluginDbContext;
            this.unitOfWork = unitOfWork;
            Version = version;
        }

        public abstract string UpScripts { get; }

        public abstract string DownScripts { get; }

        public Version Version { get; }

        protected async Task SQLAsync(string sql)
        {
            var conn = pluginDbContext.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            await conn.ExecuteAsync(sql);
        }

        public async Task MigrationUpAsync(Guid pluginId)
        {
            await SQLAsync(UpScripts);
            await WriteMigrationScriptsAsync(pluginId);
        }

        public async Task MigrationDownAsync(Guid pluginId)
        {
            await SQLAsync(DownScripts);
            await RemoveMigrationScriptsAsync(pluginId);
        }
        
        protected async Task RemoveMigrationScriptsAsync(Guid pluginId)
        {
            var plugins = await pluginDbContext.Plugins.Where(o => o.PluginId == pluginId).ToListAsync();
            foreach (var plugin in plugins)
            {
                var migrations = await pluginDbContext.PluginMigrations.Where(o => o.Plugin == plugin && o.Version == Version.VersionNumber).ToArrayAsync();
                pluginDbContext.PluginMigrations.RemoveRange(migrations);
            }
        }

        protected async Task WriteMigrationScriptsAsync(Guid pluginId)
        {
            var migration = new PluginMigrationViewModel
            {
                PluginMigrationId = Guid.NewGuid(),
                Plugin = await pluginDbContext.Plugins.FirstOrDefaultAsync(o => o.PluginId == pluginId),
                Version = Version.VersionNumber,
                Up = UpScripts,
                Down = DownScripts,
            };
            pluginDbContext.PluginMigrations.Add(migration);
            await unitOfWork.SaveAsync();
        }
    }
}
