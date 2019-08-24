using Mystique.Core.Contracts;
using Mystique.Core.Helpers;
using Mystique.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Mystique.Core.DomainModel
{
    public abstract class BaseMigration : IMigration
    {
        private Version _version = null;
        private DbHelper _dbHelper = null;

        public BaseMigration(DbHelper dbHelper, Version version)
        {
            this._version = version;
            this._dbHelper = dbHelper;
        }

        public Version Version
        {
            get
            {
                return _version;
            }
        }

        protected void SQL(string sql)
        {
            _dbHelper.ExecuteNonQuery(sql);
        }

        public abstract void MigrationDown(Guid pluginId);

        public abstract void MigrationUp(Guid pluginId);

        protected void RemoveMigrationScripts(Guid pluginId)
        {
            var sql = "DELETE PluginMigrations WHERE PluginId = @pluginId AND Version = @version";

            _dbHelper.ExecuteNonQuery(sql, new List<SqlParameter>
            {
                new SqlParameter{ ParameterName = "@pluginId", SqlDbType = SqlDbType.UniqueIdentifier, Value = pluginId },
                new SqlParameter{ ParameterName = "@version", SqlDbType = SqlDbType.NVarChar, Value = _version.VersionNumber }
            }.ToArray());
        }

        protected void WriteMigrationScripts(Guid pluginId, string up, string down)
        {
            var sql = "INSERT INTO PluginMigrations(PluginMigrationId, PluginId, Version, Up, Down) VALUES(@pluginMigrationId, @pluginId, @version, @up, @down)";

            _dbHelper.ExecuteNonQuery(sql, new List<SqlParameter>
            {
                new SqlParameter{ ParameterName = "@pluginMigrationId", SqlDbType = SqlDbType.UniqueIdentifier, Value = Guid.NewGuid() },
                new SqlParameter{ ParameterName = "@pluginId", SqlDbType = SqlDbType.UniqueIdentifier, Value = pluginId },
                new SqlParameter{ ParameterName = "@version", SqlDbType = SqlDbType.NVarChar, Value = _version.VersionNumber },
                new SqlParameter{ ParameterName = "@up", SqlDbType = SqlDbType.NVarChar, Value = up},
                new SqlParameter{ ParameterName = "@down", SqlDbType = SqlDbType.NVarChar, Value = down}
            }.ToArray());
        }
    }
}
