namespace Mystique.Core.Repositories
{
    public interface IUnitOfWork
    {
        IPluginRepository PluginRepository { get; }

        void Commit();
    }
}
