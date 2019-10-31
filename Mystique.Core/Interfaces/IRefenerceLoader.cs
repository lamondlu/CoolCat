using System.Reflection;

namespace Mystique.Core.Interfaces
{
    public interface IReferenceLoader
    {
        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context, string moduleFolder, Assembly assembly);
    }
}
