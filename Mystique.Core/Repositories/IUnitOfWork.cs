using System.Threading.Tasks;

namespace Mystique.Core.Repositories
{
    public interface IUnitOfWork
    {
        Task<bool> SaveAsync();
    }
}
