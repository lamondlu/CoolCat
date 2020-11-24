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
       

        public string Query(string moduleName, string queryName, string parameter, Func<string, string> query)
        {
            throw new NotImplementedException();
        }

        public string Query(string moduleName, string queryName, string parameter)
        {
            throw new NotImplementedException();
        }

        public void RegisterQuery(string moduleName, string queryName, string parameter)
        {
            throw new NotImplementedException();
        }

        public void RegisterQuery(string moduleName, string queryName, string parameter, Func<string, string> query)
        {
            throw new NotImplementedException();
        }
    }
}
