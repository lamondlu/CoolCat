using MySql.Data.MySqlClient;
using Mystique.Core.DTOs;
using Mystique.Core.Helpers;
using Mystique.Core.Repositories;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Mystique.Core.Repository.MySql
{
    public class PluginRepository : IPluginRepository
    {
        private readonly DbHelper _dbHelper = null;
        private readonly List<Command> _commands = null;

        public PluginRepository(DbHelper dbHelper, List<Command> commands)
        {
            _dbHelper = dbHelper;
            _commands = commands;
        }

        public void AddPlugin(AddPluginDTO dto)
        {
            Command command = new Command
            {
                Parameters = new List<MySqlParameter>(),
                Sql = "INSERT INTO Plugins(PluginId, Name, UniqueKey, Version, DisplayName,Enable) values(@pluginId, @name, @uniqueKey, @version, @displayName, @enable)"
            };

            command.Parameters.Add(new MySqlParameter { ParameterName = "@pluginId", MySqlDbType = MySqlDbType.Guid, Value = dto.PluginId });
            command.Parameters.Add(new MySqlParameter { ParameterName = "@name", MySqlDbType = MySqlDbType.VarChar, Value = dto.Name });

            command.Parameters.Add(new MySqlParameter { ParameterName = "@uniqueKey", MySqlDbType = MySqlDbType.VarChar, Value = dto.UniqueKey });

            command.Parameters.Add(new MySqlParameter { ParameterName = "@version", MySqlDbType = MySqlDbType.VarChar, Value = dto.Version });

            command.Parameters.Add(new MySqlParameter { ParameterName = "@displayName", MySqlDbType = MySqlDbType.VarChar, Value = dto.DisplayName });

            command.Parameters.Add(new MySqlParameter { ParameterName = "@enable", MySqlDbType = MySqlDbType.Bit, Value = false });

            _commands.Add(command);
        }

        public void UpdatePluginVersion(Guid pluginId, string version)
        {
            Command comand =new Command();
            

            Command command = new Command
            {
                Parameters = new List<MySqlParameter>(),
                Sql = "UPDATE Plugins SET Version = @version WHERE PluginId = @pluginId"
            };


            command.Parameters.Add(new MySqlParameter { ParameterName = "@pluginId", MySqlDbType = MySqlDbType.Guid, Value = pluginId });
            command.Parameters.Add(new MySqlParameter { ParameterName = "@version", MySqlDbType = MySqlDbType.VarChar, Value = version });

            _commands.Add(command);
        }

        public List<PluginListItemViewModel> GetAllPlugins()
        {
            List<PluginListItemViewModel> plugins = new List<PluginListItemViewModel>();
            string sql = "SELECT * from Plugins";

            DataTable table = _dbHelper.ExecuteDataTable(sql);

            foreach (DataRow row in table.Rows.Cast<DataRow>())
            {
                PluginListItemViewModel plugin = new PluginListItemViewModel
                {
                    PluginId = Guid.Parse(row["PluginId"].ToString()),
                    Name = row["Name"].ToString(),
                    UniqueKey = row["UniqueKey"].ToString(),
                    Version = row["Version"].ToString(),
                    DisplayName = row["DisplayName"].ToString(),
                    IsEnable = Convert.ToBoolean(row["Enable"])
                };

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
            string sql = "UPDATE Plugins SET Enable=@enable WHERE PluginId = @pluginId";

            _dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter> {
                new MySqlParameter{ParameterName = "@enable", MySqlDbType = MySqlDbType.Bit, Value= enable},
                new MySqlParameter{ParameterName = "@pluginId", MySqlDbType = MySqlDbType.Guid, Value= pluginId}
             }.ToArray());
        }

        public PluginViewModel GetPlugin(string pluginName)
        {
            string sql = "SELECT * from Plugins where Name = @pluginName";

            DataTable table = _dbHelper.ExecuteDataTable(sql, new MySqlParameter
            {
                ParameterName = "@pluginName",
                Value = pluginName,
                MySqlDbType = MySqlDbType.VarChar
            });

            if (table.Rows.Cast<DataRow>().Count() == 0)
            {
                return null;
            }

            DataRow row = table.Rows.Cast<DataRow>().First();

            PluginViewModel plugin = new PluginViewModel
            {
                PluginId = Guid.Parse(row["PluginId"].ToString()),
                Name = row["Name"].ToString(),
                UniqueKey = row["UniqueKey"].ToString(),
                Version = row["Version"].ToString(),
                DisplayName = row["DisplayName"].ToString(),
                IsEnable = Convert.ToBoolean(row["Enable"])
            };

            return plugin;
        }

        public PluginViewModel GetPlugin(Guid pluginId)
        {
            string sql = "SELECT * from Plugins where PluginId = @pluginId";

            DataTable table = _dbHelper.ExecuteDataTable(sql, new MySqlParameter
            {
                ParameterName = "@pluginId",
                Value = pluginId,
                MySqlDbType = MySqlDbType.Guid
            });

            if (table.Rows.Cast<DataRow>().Count() == 0)
            {
                throw new Exception("The plugin is missing in the system.");
            }

            DataRow row = table.Rows.Cast<DataRow>().First();

            PluginViewModel plugin = new PluginViewModel
            {
                PluginId = Guid.Parse(row["PluginId"].ToString()),
                Name = row["Name"].ToString(),
                UniqueKey = row["UniqueKey"].ToString(),
                Version = row["Version"].ToString(),
                DisplayName = row["DisplayName"].ToString(),
                IsEnable = Convert.ToBoolean(row["Enable"])
            };

            return plugin;

        }

        public void DeletePlugin(Guid pluginId)
        {
            string sqlPluginMigrations = "DELETE PluginMigrations where PluginId = @pluginId";

            _dbHelper.ExecuteNonQuery(sqlPluginMigrations, new List<MySqlParameter>{new MySqlParameter
            {
                ParameterName = "@pluginId",
                Value = pluginId,
                MySqlDbType = MySqlDbType.Guid
            } }.ToArray());

            string sqlPlugins = "DELETE Plugins where PluginId = @pluginId";

            _dbHelper.ExecuteNonQuery(sqlPlugins, new List<MySqlParameter>{new MySqlParameter
            {
                ParameterName = "@pluginId",
                Value = pluginId,
                MySqlDbType = MySqlDbType.Guid
            } }.ToArray());
        }

        public void RunDownMigrations(Guid pluginId)
        {
            string sql = "SELECT Down from PluginMigrations WHERE PluginId = @pluginId ORDER BY [Version] DESC";

            DataTable table = _dbHelper.ExecuteDataTable(sql, new MySqlParameter
            {
                ParameterName = "@pluginId",
                Value = pluginId,
                MySqlDbType = MySqlDbType.Guid
            });

            foreach (DataRow item in table.Rows.Cast<DataRow>())
            {
                string script = item[0].ToString();

                _dbHelper.ExecuteNonQuery(script);
            }
        }
    }
}
