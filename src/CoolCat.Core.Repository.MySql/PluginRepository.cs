using CoolCat.Core.Contracts;
using CoolCat.Core.DTOs;
using CoolCat.Core.Repositories;
using CoolCat.Core.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace CoolCat.Core.Repository.MySql
{
    public class PluginRepository : IPluginRepository
    {
        private readonly IDbConnection _dbConnection = null;
        private readonly List<Command> _commands = null;

        public PluginRepository(IDbConnection dbConnection, List<Command> commands)
        {
            _dbConnection = dbConnection;
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

            return _dbConnection.Query<PluginListItemViewModel>(sql).ToList();

            //foreach (DataRow row in table.Rows.Cast<DataRow>())
            //{
            //    PluginListItemViewModel plugin = new PluginListItemViewModel
            //    {
            //        PluginId = Guid.Parse(row["PluginId"].ToString()),
            //        Name = row["Name"].ToString(),
            //        UniqueKey = row["UniqueKey"].ToString(),
            //        Version = row["Version"].ToString(),
            //        DisplayName = row["DisplayName"].ToString(),
            //        IsEnable = Convert.ToBoolean(row["Enable"])
            //    };

            //    plugins.Add(plugin);
            //}

            //return plugins;
        }

        public List<PluginListItemViewModel> GetAllEnabledPlugins()
        {
            return GetAllPlugins().Where(p => p.IsEnable).ToList();
        }

        public void SetPluginStatus(Guid pluginId, bool enable)
        {
            string sql = "UPDATE Plugins SET Enable=@enable WHERE PluginId = @pluginId";

            _dbConnection.Execute(sql, new
            {
                enable,
                pluginId
            });

            //_dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter> {
            //    new MySqlParameter{ParameterName = "@enable", MySqlDbType = MySqlDbType.Bit, Value= enable},
            //    new MySqlParameter{ParameterName = "@pluginId", MySqlDbType = MySqlDbType.Guid, Value= pluginId}
            // }.ToArray());
        }

        public PluginViewModel GetPlugin(string pluginName)
        {
            string sql = "SELECT * from Plugins where Name = @pluginName";

            var plugin = _dbConnection.QueryFirstOrDefault<PluginViewModel>(sql, new { pluginName });

            if (plugin == null)
            {
                return null;
            }

            return plugin;
        }

        public PluginViewModel GetPlugin(Guid pluginId)
        {
            string sql = "SELECT * from Plugins where PluginId = @pluginId";

            var plugin = _dbConnection.QueryFirstOrDefault<PluginViewModel>(sql, new { pluginId });

            if (plugin == null)
            {
                throw new Exception("The plugin is missing in the system.");
            }

            return plugin;

        }

        public void DeletePlugin(Guid pluginId)
        {
            string sqlPluginMigrations = "DELETE PluginMigrations where PluginId = @pluginId";

            _dbConnection.Execute(sqlPluginMigrations, new { pluginId });

            string sqlPlugins = "DELETE Plugins where PluginId = @pluginId";

            _dbConnection.Execute(sqlPlugins, new { pluginId });
        }

        public void RunDownMigrations(Guid pluginId)
        {
            string sql = "SELECT Down from PluginMigrations WHERE PluginId = @pluginId ORDER BY [Version] DESC";

            var scripts = _dbConnection.Query<string>(sql, new { pluginId });

            foreach (var script in scripts)
            {
                _dbConnection.Execute(script);
            }
        }
    }
}
