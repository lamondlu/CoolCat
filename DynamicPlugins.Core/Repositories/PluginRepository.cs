using System;
using System.Collections.Generic;
using System.Text;
using DynamicPlugins.Core.Helpers;
using DynamicPlugins.Core.ViewModels;
using System.Linq;
using System.Data;
using DynamicPlugins.Core.DTOs;

namespace DynamicPlugins.Core.Repositories
{
    public class PluginRepository : IPluginRepository
    {
        private DbHelper _dbHelper = null;
        private List<Command> _commands = null;

        public PluginRepository(DbHelper dbHelper, List<Command> commands)
        {
            _dbHelper = dbHelper;
            _commands = commands;
        }

        public void AddPlugin(AddPluginDTO dto)
        {
            var command = new Command();
            command.Sql = "INSERT INTO Plugin(PluginId, Name, UniqueKey, Version, DisplayName, DLLPath, ViewDLLPath) values(@pluginId, @name, @uniqueKey, @version, @displayName, @dllPath, @viewDllPath)";

            _commands.Add(command);
        }

        public List<PluginListItemViewModel> GetAllPlugins()
        {
            var plugins = new List<PluginListItemViewModel>();
            var sql = "SELECT * from Plugins";

            var table = _dbHelper.ExecuteDataTable(sql);

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
