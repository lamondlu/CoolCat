using Mystique.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookInventory.DataStores
{
    public class AvailableBookQuery : IDataStoreQuery
    {
        public string QueryName
        {
            get
            {
                return "Available_Books";
            }
        }
        public Func<string, string> Query
        {
            get
            {
                return null;
            }
        }

      
    }
}
