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
            command.Sql = "INSERT INTO Plugins(PluginId, Name, UniqueKey, Version, DisplayName,Enable) values(@pluginId, @name, @uniqueKey, @version, @displayName, @enable)";

            command.Parameters.Add(new SqlParameter { ParameterName = "@pluginId", SqlDbType = SqlDbType.UniqueIdentifier, Value = dto.PluginId });
            command.Parameters.Add(new SqlParameter { ParameterName = "@name", SqlDbType = SqlDbType.NVarChar, Value = dto.Name });

            command.Parameters.Add(new SqlParameter { ParameterName = "@uniqueKey", SqlDbType = SqlDbType.NVarChar, Value = dto.UniqueKey });

            command.Parameters.Add(new SqlParameter { ParameterName = "@version", SqlDbType = SqlDbType.NVarChar, Value = dto.Version });

            command.Parameters.Add(new SqlParameter { ParameterName = "@displayName", SqlDbType = SqlDbType.NVarChar, Value = dto.DisplayName });

            command.Parameters.Add(new SqlParameter { ParameterName = "@enable", SqlDbType = SqlDbType.Bit, Value = false });

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
                plugin.IsEnable = Convert.ToBoolean(row["Enable"]);

                plugins.Add(plugin);
            }

            return plugins;
        }

        public List<PluginListItemViewModel> GetAllEnabledPlugins()
        {
            return GetAllPlugins().Where(p => p.IsEnable).ToList();
        }

        public void SetPluginStatus(Guid pluginId, bool enable)
        {
            var sql = "UPDATE Plugins SET Enable=@enable WHERE PluginId = @pluginId";

            _dbHelper.ExecuteNonQuery(sql, new List<SqlParameter> {
                new SqlParameter{ParameterName = "@enable", SqlDbType = SqlDbType.Bit, Value= enable},
                new SqlParameter{ParameterName = "@pluginId", SqlDbType = SqlDbType.UniqueIdentifier, Value= pluginId}
             }.ToArray());
        }

        public PluginViewModel GetPlugin(Guid pluginId)
        {
            var sql = "SELECT * from Plugins where PluginId = @pluginId";

            var table = _dbHelper.ExecuteDataTable(sql, new SqlParameter
            {
                ParameterName = "@pluginId",
                Value = pluginId,
                SqlDbType = SqlDbType.UniqueIdentifier
            });

            if (table.Rows.Cast<DataRow>().Count() == 0)
            {
                throw new Exception("The plugin is missing in the system.");
            }

            var row = table.Rows.Cast<DataRow>().First();

            var plugin = new PluginViewModel();
            plugin.PluginId = Guid.Parse(row["PluginId"].ToString());
            plugin.Name = row["Name"].ToString();
            plugin.UniqueKey = row["UniqueKey"].ToString();
            plugin.Version = row["Version"].ToString();
            plugin.DisplayName = row["DisplayName"].ToString();
            plugin.IsEnable = Convert.ToBoolean(row["Enable"]);

            return plugin;

        }

        public void DeletePlugin(Guid pluginId)
        {
            var sqlPluginMigrations = "DELETE PluginMigrations where PluginId = @pluginId";

            _dbHelper.ExecuteNonQuery(sqlPluginMigrations, new List<SqlParameter>{new SqlParameter
            {
                ParameterName = "@pluginId",
                Value = pluginId,
                SqlDbType = SqlDbType.UniqueIdentifier
            } }.ToArray());

            var sqlPlugins = "DELETE Plugins where PluginId = @pluginId";

            _dbHelper.ExecuteNonQuery(sqlPlugins, new List<SqlParameter>{new SqlParameter
            {
                ParameterName = "@pluginId",
                Value = pluginId,
                SqlDbType = SqlDbType.UniqueIdentifier
            } }.ToArray());
        }

        public void RunDownMigrations(Guid pluginId)
        {
            var sql = "SELECT Down from PluginMigrations WHERE PluginId = @pluginId ORDER BY [Version] DESC";

            var table = _dbHelper.ExecuteDataTable(sql, new SqlParameter
            {
                ParameterName = "@pluginId",
                Value = pluginId,
                SqlDbType = SqlDbType.UniqueIdentifier
            });

            foreach (var item in table.Rows.Cast<DataRow>())
            {
                var script = item[0].ToString();

                _dbHelper.ExecuteNonQuery(script);
            }
        }
    }
}
