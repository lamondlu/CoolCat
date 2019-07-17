using System;
using System.Collections.Generic;
using System.Text;
using DynamicPlugins.Core.Helpers;
using DynamicPlugins.Core.ViewModels;
using System.Linq;
using System.Data;

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
            var plugins = new List<PluginListItemViewModel>();
            var sql = "SELECT * from Plugins";

            var table = _dbHelper.ExecuteTable(sql);

            foreach (var row in table.Rows.Cast<DataRow>())
            {
                var plugin = new PluginListItemViewModel();
                plugin.PluginId = Guid.Parse(row["PluginId"].ToString());
                plugin.Name = row["Name"].ToString();
                plugin.UniqueKey = row["UniqueKey"].ToString();
                plugin.Version = row["Version"].ToString();
                plugin.DisplayName = row["DisplayName"].ToString();

                plugins.Add(plugin);
            }

            return plugins;
        }
    }
}
