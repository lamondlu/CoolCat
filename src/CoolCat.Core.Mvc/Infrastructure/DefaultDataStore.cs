using CoolCat.Core.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public class DefaultDataStore : IDataStore
    {
        private ILogger<DefaultDataStore> _logger = null;

        public DefaultDataStore(ILogger<DefaultDataStore> logger)
        {
            _logger = logger;
        }

        private List<QueryItem> _queryItems = new List<QueryItem>();

        public string Query(string moduleName, string queryName, string parameter, string source = "")
        {
            var query = _queryItems.FirstOrDefault(p => p.ModuleName == moduleName && p.QueryName == queryName);

            if (query != null)
            {
                _logger.LogDebug($"Module '{source}' try to query the '{queryName}' from '{moduleName}' with parameter '{parameter}'");

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
