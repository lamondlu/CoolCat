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
}
