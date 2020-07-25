namespace Mystique.Core.Contracts
{
    public interface ISystemManager
    {
        bool CheckInstall();

        void MarkAsInstalled();
    }
}
