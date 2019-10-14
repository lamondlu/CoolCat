using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Mystique.Core.ViewModels
{
    public class PluginViewModel
    {
        public Guid PluginId { get; set; }
        public string UniqueKey { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Version { get; set; }
        public bool IsEnable { get; set; }
        public List<PluginMigrationViewModel> PluginMigrations { get; set; }
    }

    public class PluginViewModelConfiguration : IEntityTypeConfiguration<PluginViewModel>
    {
        public void Configure(EntityTypeBuilder<PluginViewModel> builder)
        {
            builder.ToTable("Plugins");
            builder.HasKey(o => o.PluginId);
            builder.HasMany(o => o.PluginMigrations).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PluginMigrationViewModel
    {
        public Guid PluginMigrationId { get; set; }
        public PluginViewModel Plugin { get; set; }
        public string Version { get; set; }
        public string Up { get; set; }
        public string Down { get; set; }
    }

    public class PluginMigrationViewModelConfiguration : IEntityTypeConfiguration<PluginMigrationViewModel>
    {
        public void Configure(EntityTypeBuilder<PluginMigrationViewModel> builder)
        {
            builder.ToTable("PluginMigrations");
            builder.HasKey(o => o.PluginMigrationId);
            builder.HasOne(o => o.Plugin).WithMany(o => o.PluginMigrations).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
