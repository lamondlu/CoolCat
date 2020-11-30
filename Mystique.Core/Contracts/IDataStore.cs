using System;

namespace Mystique.Core.Contracts
{
    public interface IDataStore
    {
        string Query(string moduleName, string queryName, string parameter, string source = "");

        void RegisterQuery(string moduleName, string queryName, Func<string, string> query);
    }
}
