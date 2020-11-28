using Microsoft.Extensions.Options;
using Mystique.Core.Contracts;
using Mystique.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class DefaultDataStore : IDataStore
    {
        private string _connectionString = string.Empty;

        public DefaultDataStore(IOptions<ConnectionStringSetting> connectionStringAccessor)
        {
            _connectionString = connectionStringAccessor.Value.ConnectionString;
        }

        private List<QueryItem> _queryItems = new List<QueryItem>();

        public string Query(string moduleName, string queryName, string parameter)
        {
            var query = _queryItems.FirstOrDefault(p => p.ModuleName == moduleName && p.QueryName == queryName);

            if (query != null)
            {
                return query.Query(parameter);
            }
            else
            {
                return string.Empty;
            }
        }

        public void RegisterQuery(string moduleName, string queryName, Func<string, string> query)
        {
            _queryItems.Add(new QueryItem
            {
                ModuleName = moduleName,
                QueryName = queryName,
                Query = query
            });
        }
    }

    public class QueryItem
    {
        public string ModuleName { get; set; }

        public string QueryName { get; set; }

        public Func<string, string> Query { get; set; }

        public string Run(string parameter)
        {
            return Query(parameter);
        }
    }
}
