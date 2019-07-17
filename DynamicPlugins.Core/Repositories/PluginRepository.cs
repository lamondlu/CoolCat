using System;
using System.Collections.Generic;
using System.Text;
using DynamicPlugins.Core.Helpers;
using DynamicPlugins.Core.ViewModels;

namespace DynamicPlugins.Core.Repositories
{
    public class PluginRepository : IPluginRepository
    {
        private DbHelper _dbHelper = null;

        public PluginRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<PluginListItemViewModel> GetAllPlugins()
        {
            var sql = "SELECT * from Plugins";

            throw new NotImplementedException();
        }
    }
}
