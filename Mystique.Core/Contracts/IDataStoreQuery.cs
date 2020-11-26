using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystique.Core.Contracts
{
    public interface IDataStoreQuery
    {
        string QueryName { get; }

        string Query(string parameter);
    }
}
