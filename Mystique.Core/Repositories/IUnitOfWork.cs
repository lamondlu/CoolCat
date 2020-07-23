namespace Mystique.Core.Repositories
{
    public interface IUnitOfWork
    {
        IPluginRepository PluginRepository { get; }


        bool CheckDatabase();

        void MarkAsInstalled();

        void Commit();
    }
}
