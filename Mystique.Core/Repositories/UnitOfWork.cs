using Mystique.Core.Helpers;
using Mystique.Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace Mystique.Core.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbHelper _dbHelper = null;
        private string _connectionString = string.Empty;
        private IPluginRepository _pluginRepository = null;
        private List<Command> _commands;

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
    }
}
