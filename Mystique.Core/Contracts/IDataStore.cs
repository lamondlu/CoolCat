using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystique.Core.Contracts
{
    public interface IDataStore
    {
        string Query(string moduleName, string queryName, string parameter);

        void RegisterQuery(string moduleName, string queryName, Func<string, string> query);
    }
}
