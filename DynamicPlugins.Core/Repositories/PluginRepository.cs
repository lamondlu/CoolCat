using System;
using System.Collections.Generic;
using System.Text;
using DynamicPlugins.Core.Helpers;

namespace DynamicPlugins.Core.Repositories
{
    public class PluginRepository : IPluginRepository
    {
        private DbHelper _dbHelper = null;

        public PluginRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }


    }
}
