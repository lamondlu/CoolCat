using Mystique.Core.Contracts;
using Mystique.Core.DTOs;
using Mystique.Core.Repositories;
using Mystique.Core.ViewModels;

namespace Mystique.Core.BusinessLogic
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
