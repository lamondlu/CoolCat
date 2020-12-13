namespace CoolCat.Core.Contracts
{
    public interface IModule
    {
        string Name { get; }

        DomainModel.Version Version { get; }
    }
}
