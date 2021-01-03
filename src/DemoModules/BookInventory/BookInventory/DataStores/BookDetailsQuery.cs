using CoolCat.Core.Attributes;
using CoolCat.Core.Contracts;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using Dapper;

namespace BookInventory.DataStores
{
    [RequestParameterType(typeof(BookDetailsQueryParameter))]
    [ResponseType(typeof(BookDetailViewModel))]
    public class BookDetailsQuery : IDataStoreQuery
    {
        private IDbConnectionFactory _dbConnectionFactory = null;

        public BookDetailsQuery(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public string QueryName
        {
            get
            {
                return "Book_Details";
            }
        }

        public string Query(string parameter)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var param = JsonConvert.DeserializeObject<BookDetailsQueryParameter>(parameter);

                var sql = "SELECT * FROM Book WHERE BookId=@id";

                var items = connection.Query<BookDetailViewModel>(sql, new { id = param.BookId }).ToList();

                if (items.Count == 1)
                {
                    return JsonConvert.SerializeObject(items.First());
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public class BookDetailsQueryParameter
    {
        public Guid BookId { get; set; }
    }

    public class BookDetailViewModel
    {
        public Guid BookId { get; set; }

        public string BookName { get; set; }

        public string ISBN { get; set; }

        public DateTime DateIssued { get; set; }
    }

}
