using CoolCat.Core.DTOs;
using CoolCat.Core.ViewModels;

namespace CoolCat.Core.Contracts
{
    public interface ISystemManager
    {
        bool CheckInstall();

        void MarkAsInstalled();

        SiteSettingsViewModel GetSiteSettings();

        void SaveSiteSettings(SiteSettingsDTO dto);
    }
}
