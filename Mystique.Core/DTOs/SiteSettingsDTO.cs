using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.DTOs
{
    public class SiteSettingsDTO
    {
        public string SiteCSS { get; set; }

        public Guid? SiteTemplateId { get; set; }
    }
}
