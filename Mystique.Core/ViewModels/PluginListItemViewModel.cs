using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Mystique.Core.ViewModels
{
    public class PluginListItemViewModel
    {
        public Guid PluginId { get; set; }
        public string UniqueKey { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Version { get; set; }
        public bool IsEnable { get; set; }
    }

    public class PluginListItemViewModelConfiguration : IEntityTypeConfiguration<PluginListItemViewModel>
    {
        public void Configure(EntityTypeBuilder<PluginListItemViewModel> builder)
        {
            builder.HasKey(o => o.PluginId);
        }
    }
}
