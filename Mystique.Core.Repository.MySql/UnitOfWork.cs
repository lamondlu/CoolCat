using Microsoft.Extensions.Options;
using Mystique.Core.Models;
using Mystique.Core.Repositories;
using System;
using System.Collections.Generic;
using MySqlClient = MySql.Data.MySqlClient;

namespace Mystique.Core.Repository.MySql
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbHelper _dbHelper = null;
        private readonly string _connectionString = string.Empty;
        private IPluginRepository _pluginRepository = null;
        private readonly List<Command> _commands;

        public UnitOfWork(IOptions<ConnectionStringSetting> connectionStringAccessor)
        {
            _commands = new List<Command>();
            _connectionString = connectionStringAccessor.Value.ConnectionString;
            _dbHelper = new DbHelper(_connectionString);
        }

        public IPluginRepository PluginRepository
        {
            get
            {
                if (_pluginRepository == null)
                {
                    _pluginRepository = new PluginRepository(_dbHelper, _commands);
                }

                return _pluginRepository;
            }
        }

        public void Commit()
        {
            _dbHelper.ExecuteNonQuery(_commands);
        }

        public bool CheckDatabase()
        {
            var o = _dbHelper.ExecuteScalarWithObjReturn("SELECT `Value` FROM GlobalSettings WHERE `Key` = @key", new List<MySqlClient.MySqlParameter> {
               new MySqlClient.MySqlParameter { ParameterName = "@key", Value = "SYSTEM_INSTALLED"}
            }.ToArray());

            return (o != null && o.ToString() == "1");
        }

        public void MarkAsInstalled()
        {
            _dbHelper.ExecuteNonQuery("UPDATE GlobalSettings SET `Value`='1' WHERE `Key`=@key", new List<MySqlClient.MySqlParameter> {
               new MySqlClient.MySqlParameter { ParameterName = "@key", Value = "SYSTEM_INSTALLED"}
            }.ToArray());
        }
    }
}
