using MySql.Data.MySqlClient;
using Mystique.Core.Contracts;
using Mystique.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Mystique.Core.DomainModel
{
    public abstract class BaseMigration : IMigration
    {
        private readonly Version _version = null;
        private readonly DbHelper _dbHelper = null;

        public BaseMigration(DbHelper dbHelper, Version version)
        {
            _version = version;
            _dbHelper = dbHelper;
        }

        public Version Version => _version;

        public abstract string UpScripts
        {
            get;
        }

        public abstract string DownScripts
        {
            get;
        }

        protected void SQL(string sql)
        {
            _dbHelper.ExecuteNonQuery(sql);
        }

        public void MigrateUp(Guid pluginId)
        {
            SQL(UpScripts);
            WriteMigrationScripts(pluginId);
        }

        public void MigrateDown(Guid pluginId)
        {
            SQL(DownScripts);
            RemoveMigrationScripts(pluginId);
        }

        private void RemoveMigrationScripts(Guid pluginId)
        {
            string sql = "DELETE PluginMigrations WHERE PluginId = @pluginId AND Version = @version";

            _dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter>
            {
                new MySqlParameter{ ParameterName = "@pluginId", MySqlDbType = MySqlDbType.Guid, Value = pluginId },
                new MySqlParameter{ ParameterName = "@version", MySqlDbType = MySqlDbType.VarChar, Value = _version.VersionNumber }
            }.ToArray());

            
        }

        private void WriteMigrationScripts(Guid pluginId)
        {
            string sql = "INSERT INTO PluginMigrations(PluginMigrationId, PluginId, Version, Up, Down) VALUES(@pluginMigrationId, @pluginId, @version, @up, @down)";

            _dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter>
            {
                new MySqlParameter{ ParameterName = "@pluginMigrationId", MySqlDbType = MySqlDbType.Guid, Value = Guid.NewGuid() },
                new MySqlParameter{ ParameterName = "@pluginId", MySqlDbType = MySqlDbType.Guid, Value = pluginId },
                new MySqlParameter{ ParameterName = "@version", MySqlDbType = MySqlDbType.VarChar, Value = _version.VersionNumber },
                new MySqlParameter{ ParameterName = "@up", MySqlDbType = MySqlDbType.VarChar, Value = UpScripts},
                new MySqlParameter{ ParameterName = "@down", MySqlDbType = MySqlDbType.VarChar, Value = DownScripts}
            }.ToArray());
        }
    }
}
