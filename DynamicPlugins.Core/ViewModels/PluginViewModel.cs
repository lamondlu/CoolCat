using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.ViewModels
{
    public class PluginViewModel
    {
        public Guid PluginId { get; set; }

        public string UniqueKey { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Version { get; set; }

        public bool IsEnable { get; set; }
    }
}
