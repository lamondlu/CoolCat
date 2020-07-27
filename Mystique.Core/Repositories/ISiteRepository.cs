using Mystique.Core.DTOs;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Repositories
{
    public interface ISiteRepository
    {
        SiteSettingsViewModel GetSiteSettings();

        void SaveSiteSettings(SiteSettingsDTO dto);
    }
}
