using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.DTOs
{
    public class AddPluginDTO
    {
        public AddPluginDTO()
        {
            PluginId = Guid.NewGuid();
        }

        [JsonIgnore]
        public Guid PluginId { get; set; }

        public string UniqueKey { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Version { get; set; }

        public string DLLPath { get; set; }

        public string ViewDLLPath { get; set; }
    }
}
