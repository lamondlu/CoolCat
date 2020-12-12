namespace CoolCat.Core.Contracts
{
    public interface ICollectibleAssemblyLoadContextProvider
    {
        CollectibleAssemblyLoadContext Get(string moduleName);
    }
}
