using Mystique.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class DefaultDataStore : IDataStore
    {
        private List<QueryItem> _queryItems = new List<QueryItem>();

        public string Query(string moduleName, string queryName, string parameter)
        {
            var query = _queryItems.FirstOrDefault(p => p.ModuleName == moduleName && p.QueryName == queryName);

            if (query != null)
            {
                return query.Run(parameter);
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
