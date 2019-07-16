using DynamicPlugins.Core.Helpers;
using DynamicPlugins.Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace DynamicPlugins.Core.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbHelper _dbHelper = null;
        private string _connectionString = string.Empty;
        private IPluginRepository _pluginRepository = null;
        private List<string> _scripts;

        public UnitOfWork(IOptions<ConnectionStringSetting> connectionStringAccessor)
        {
            _scripts = new List<string>();
            _connectionString = connectionStringAccessor.Value.ConnectionString;
            _dbHelper = new DbHelper(_connectionString);
        }

        public IPluginRepository PluginRepository
        {
            get
            {
                if (_pluginRepository == null)
                {
                    _pluginRepository = new PluginRepository(_dbHelper);
                }

                return _pluginRepository;
            }
        }

        public void Commit()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var scope = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        foreach (var script in _scripts)
                        {
                            var cmd = new SqlCommand(script, sqlConnection);
                            cmd.ExecuteNonQuery();
                        }

                        scope.Commit();
                    }
                    catch
                    {
                        scope.Rollback();
                    }
                }
            }
        }
    }
}
