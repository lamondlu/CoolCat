using Microsoft.EntityFrameworkCore;
using Mystique.Core.ViewModels;
using System.Threading.Tasks;

namespace Mystique.Core.Repositories
{
    public class PluginDbContext : DbContext
    {
        public PluginDbContext(DbContextOptions<PluginDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PluginDbContext).Assembly);
        }

        public virtual DbSet<PluginViewModel> Plugins { get; set; }
        public virtual DbSet<FtpFileDetail> FtpFileDetails { get; set; }
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly PluginDbContext context;

        public UnitOfWork(PluginDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> SaveAsync() => await context.SaveChangesAsync() > 0;
    }
}
