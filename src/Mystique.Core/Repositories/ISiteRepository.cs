using Mystique.Core.DTOs;
using Mystique.Core.ViewModels;

namespace Mystique.Core.Repositories
{
    public interface ISiteRepository
    {
        SiteSettingsViewModel GetSiteSettings();

        void SaveSiteSettings(SiteSettingsDTO dto);
    }
}
