using System;
using System.Collections.Generic;
using System.Text;
using DynamicPlugins.Core.Helpers;
using DynamicPlugins.Core.ViewModels;
using System.Linq;
using System.Data;
using DynamicPlugins.Core.DTOs;
using System.Data.SqlClient;

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
            command.Parameters = new List<SqlParameter>();
            command.Sql = "INSERT INTO Plugins(PluginId, Name, UniqueKey, Version, DisplayName) values(@pluginId, @name, @uniqueKey, @version, @displayName)";

            command.Parameters.Add(new SqlParameter { ParameterName = "@pluginId", SqlDbType = SqlDbType.UniqueIdentifier, Value = dto.PluginId });
            command.Parameters.Add(new SqlParameter { ParameterName = "@name", SqlDbType = SqlDbType.UniqueIdentifier, Value = dto.Name });

            command.Parameters.Add(new SqlParameter { ParameterName = "@uniqueKey", SqlDbType = SqlDbType.UniqueIdentifier, Value = dto.UniqueKey });

            command.Parameters.Add(new SqlParameter { ParameterName = "@version", SqlDbType = SqlDbType.UniqueIdentifier, Value = dto.Version });

            command.Parameters.Add(new SqlParameter { ParameterName = "@displayName", SqlDbType = SqlDbType.UniqueIdentifier, Value = dto.DisplayName });

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
