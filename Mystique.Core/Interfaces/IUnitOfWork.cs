using System.Threading.Tasks;

namespace Mystique.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveAsync();
    }
}
