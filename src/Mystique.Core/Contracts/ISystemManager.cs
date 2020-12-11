using Mystique.Core.DTOs;
using Mystique.Core.ViewModels;

namespace Mystique.Core.Contracts
{
    public interface ISystemManager
    {
        bool CheckInstall();

        void MarkAsInstalled();

        SiteSettingsViewModel GetSiteSettings();

        void SaveSiteSettings(SiteSettingsDTO dto);
    }
}
