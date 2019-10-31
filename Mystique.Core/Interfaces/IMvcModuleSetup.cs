using System.Threading.Tasks;

namespace Mystique.Core.Interfaces
{
    public interface IMvcModuleSetup
    {
        Task DisableModuleAsync(string moduleName);
        Task EnableModuleAsync(string moduleName);
        Task DeleteModuleAsync(string moduleName);
    }
}
