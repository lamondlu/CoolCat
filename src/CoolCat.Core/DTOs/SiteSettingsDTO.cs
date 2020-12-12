using System;

namespace CoolCat.Core.DTOs
{
    public class SiteSettingsDTO
    {
        public string SiteCSS { get; set; }

        public Guid? SiteTemplateId { get; set; }
    }
}
