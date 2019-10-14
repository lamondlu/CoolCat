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

        public Version Version { get; }

        public abstract void MigrationDown(Guid pluginId);

        public abstract void MigrationUp(Guid pluginId);

        protected async Task RemoveMigrationScriptsAsync(Guid pluginId)
        {
            var plugins = await pluginDbContext.Plugins.Where(o => o.PluginId == pluginId).ToListAsync();
            foreach (var plugin in plugins)
            {
                var migrations = await pluginDbContext.PluginMigrations.Where(o => o.Plugin == plugin && o.Version == Version.VersionNumber).ToArrayAsync();
                pluginDbContext.PluginMigrations.RemoveRange(migrations);
            }
        }

        protected async Task WriteMigrationScriptsAsync(Guid pluginId, string up, string down)
        {
            var migration = new PluginMigrationViewModel
            {
                PluginMigrationId = Guid.NewGuid(),
                Plugin = await pluginDbContext.Plugins.AsNoTracking().FirstOrDefaultAsync(o => o.PluginId == pluginId),
                Version = Version.VersionNumber,
                Up = up,
                Down = down,
            };
            pluginDbContext.PluginMigrations.Add(migration);
            await unitOfWork.SaveAsync();
        }
    }
}
