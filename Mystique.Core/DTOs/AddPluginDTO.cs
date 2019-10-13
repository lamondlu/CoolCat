using Newtonsoft.Json;
using System;

namespace Mystique.Core.DTOs
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
    }
}
