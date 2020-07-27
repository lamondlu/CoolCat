using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.ViewModels
{
    public class SiteSettingsViewModel
    {
        public string SiteCSS { get; set; }

        public Guid? SiteTemplateId { get; set; }
    }
}
