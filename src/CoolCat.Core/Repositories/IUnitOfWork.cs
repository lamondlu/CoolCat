namespace CoolCat.Core.Repositories
{
    public interface IUnitOfWork
    {
        IPluginRepository PluginRepository { get; }

        ISiteRepository SiteRepository { get; }


        bool CheckDatabase();

        void MarkAsInstalled();


        void Begin();

        void RollBack();


        void Commit();
    }
}
