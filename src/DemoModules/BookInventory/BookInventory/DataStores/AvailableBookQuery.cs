using CoolCat.Core.Attributes;
using CoolCat.Core.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace BookInventory.DataStores
{
    [NoneRequestParameter]
    [ResponseType(typeof(List<AvailableBookViewModel>))]
    public class AvailableBookQuery : IDataStoreQuery
    {
        private IDbConnectionFactory _dbConnectionFactory = null;

        public AvailableBookQuery(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
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
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM Book WHERE Status=0";
                var books = connection.Query<AvailableBookViewModel>(sql).ToList();
                return JsonConvert.SerializeObject(books);
            }
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
