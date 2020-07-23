using Mystique.Core.Contracts;
using Mystique.Core.Repositories;

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
    }
}
