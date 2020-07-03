using Mystique.Core.Repositories;

namespace Mystique.Core.BusinessLogic
{
    public class SystemManager
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
    }
}
