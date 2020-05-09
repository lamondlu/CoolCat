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

            _dbHelper.ExecuteNonQuery(sql, new List<SqlParameter>
            {
                new SqlParameter{ ParameterName = "@pluginId", SqlDbType = SqlDbType.UniqueIdentifier, Value = pluginId },
                new SqlParameter{ ParameterName = "@version", SqlDbType = SqlDbType.NVarChar, Value = _version.VersionNumber }
            }.ToArray());
        }

        private void WriteMigrationScripts(Guid pluginId)
        {
            string sql = "INSERT INTO PluginMigrations(PluginMigrationId, PluginId, Version, Up, Down) VALUES(@pluginMigrationId, @pluginId, @version, @up, @down)";

            _dbHelper.ExecuteNonQuery(sql, new List<SqlParameter>
            {
                new SqlParameter{ ParameterName = "@pluginMigrationId", SqlDbType = SqlDbType.UniqueIdentifier, Value = Guid.NewGuid() },
                new SqlParameter{ ParameterName = "@pluginId", SqlDbType = SqlDbType.UniqueIdentifier, Value = pluginId },
                new SqlParameter{ ParameterName = "@version", SqlDbType = SqlDbType.NVarChar, Value = _version.VersionNumber },
                new SqlParameter{ ParameterName = "@up", SqlDbType = SqlDbType.NVarChar, Value = UpScripts},
                new SqlParameter{ ParameterName = "@down", SqlDbType = SqlDbType.NVarChar, Value = DownScripts}
            }.ToArray());
        }
    }
}
