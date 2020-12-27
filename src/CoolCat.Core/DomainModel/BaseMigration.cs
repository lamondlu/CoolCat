using CoolCat.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;

namespace CoolCat.Core.DomainModel
{
    public abstract class BaseMigration : IMigration
    {
        private readonly Version _version = null;
        private readonly IDbConnection _dbConnection = null;

        public BaseMigration(Version version, IDbConnectionFactory dbConnectionFactory)
        {
            _version = version;
            _dbConnection = dbConnectionFactory.GetConnection();
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
            _dbConnection.Execute(sql);
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

            _dbConnection.Execute(sql, new
            {
                pluginId,
                version = _version.VersionNumber
            });
        }

        private void WriteMigrationScripts(Guid pluginId)
        {
            string sql = @"INSERT INTO PluginMigrations(PluginMigrationId, PluginId, Version, Up, Down) 
                           VALUES(@pluginMigrationId, @pluginId, @version, @up, @down)";

            _dbConnection.Execute(sql, new
            {
                pluginMigrationId = Guid.NewGuid(),
                pluginId,
                version = _version.VersionNumber,
                up = UpScripts,
                down = DownScripts
            });
        }
    }
}
