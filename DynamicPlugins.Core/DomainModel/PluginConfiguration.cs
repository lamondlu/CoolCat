using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.DomainModel
{
    public class PluginConfiguration
    {
        public string Name { get; set; }

        public string UniqueKey { get; set; }

        public string DisplayName { get; set; }

        public string Version { get; set; }
    }
}
