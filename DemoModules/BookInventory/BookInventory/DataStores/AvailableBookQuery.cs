using Mystique.Core.Contracts;
using Mystique.Core.Repository.MySql;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;

namespace BookInventory.DataStores
{
    public class AvailableBookQuery : IDataStoreQuery
    {
        private IDbHelper _dbHelper = null;

        public AvailableBookQuery(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
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
            var sql = "SELECT * FROM Book WHERE Status=0";

            var dataTable = _dbHelper.ExecuteDataTable(sql);

            return JsonConvert.SerializeObject(dataTable.Rows.Cast<DataRow>().Select(p => new AvailableBookViewModel
            {
                BookId = Guid.Parse(p["BookId"].ToString()),
                BookName = p["BookName"].ToString(),
                ISBN = p["ISBN"].ToString(),
                DateIssued = Convert.ToDateTime(p["DateIssued"])
            }).ToList());
        }
    }

    public class AvailableBookViewModel
    {
        public Guid BookId { get; set; }

        public string BookName { get; set; }

        public string ISBN { get; set; }

        public DateTime DateIssued { get; set; }
    }

}
