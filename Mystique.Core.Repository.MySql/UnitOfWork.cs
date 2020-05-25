using Microsoft.Extensions.Options;
using Mystique.Core.Helpers;
using Mystique.Core.Models;
using Mystique.Core.Repositories;
using System.Collections.Generic;

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
    }
}
