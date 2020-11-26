using Mystique.Core.Contracts;
using Mystique.Core.Repository.MySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookInventory.DataStores
{
    public class AvailableBookQuery : IDataStoreQuery
    {
        private DbHelper _dbHelper = null;

        public AvailableBookQuery(string connectionString)
        {
            _dbHelper = new DbHelper(connectionString);
        }

        public string QueryName
        {
            get
            {
                return "Available_Books";
            }
        }

        public string Query(string parameter)
        {
            return null;
        }
    }
}
