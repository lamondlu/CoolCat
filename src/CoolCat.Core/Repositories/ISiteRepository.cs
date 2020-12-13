using CoolCat.Core.DTOs;
using CoolCat.Core.ViewModels;

namespace CoolCat.Core.Repositories
{
    public interface ISiteRepository
    {
        SiteSettingsViewModel GetSiteSettings();

        void SaveSiteSettings(SiteSettingsDTO dto);
    }
}
