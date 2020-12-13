namespace CoolCat.Core.Contracts
{
    public interface IDataStoreQuery
    {
        string QueryName { get; }

        string Query(string parameter);
    }
}
