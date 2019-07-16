using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Helpers
{
    public class DbHelper
    {
        private string _connectionString = string.Empty;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
