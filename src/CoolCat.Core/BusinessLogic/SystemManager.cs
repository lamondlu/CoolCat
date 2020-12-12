using CoolCat.Core.Contracts;
using CoolCat.Core.DTOs;
using CoolCat.Core.Repositories;
using CoolCat.Core.ViewModels;

namespace CoolCat.Core.BusinessLogic
{
    public class SystemManager : ISystemManager
    {
        private readonly IUnitOfWork _unitOfWork = null;

        public SystemManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckInstall()
        {
            return _unitOfWork.CheckDatabase();
        }

        public void MarkAsInstalled()
        {
            _unitOfWork.MarkAsInstalled();
            _unitOfWork.Commit();
        }

        public SiteSettingsViewModel GetSiteSettings()
        {
            return _unitOfWork.SiteRepository.GetSiteSettings();
        }

        public void SaveSiteSettings(SiteSettingsDTO dto)
        {
            _unitOfWork.SiteRepository.SaveSiteSettings(dto);
            _unitOfWork.Commit();
        }
    }
}
