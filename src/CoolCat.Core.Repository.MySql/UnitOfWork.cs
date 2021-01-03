using CoolCat.Core.Contracts;
using CoolCat.Core.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace CoolCat.Core.Repository.MySql
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbConnectionFactory _dbConnectionFactory = null;
        private IDbConnection _dbConnection = null;
        private IPluginRepository _pluginRepository = null;
        private ISiteRepository _siteRepository = null;
        private IDbTransaction _transactionScope = null;
        private readonly List<Command> _commands;

        public UnitOfWork(IDbConnectionFactory dbConnectionFactory)
        {
            _commands = new List<Command>();
            _dbConnectionFactory = dbConnectionFactory;
            _dbConnection = _dbConnectionFactory.GetConnection();
            _dbConnection.Open();
        }



        public IPluginRepository PluginRepository
        {
            get
            {
                if (_pluginRepository == null)
                {
                    _pluginRepository = new PluginRepository(_dbConnection, _commands);
                }

                return _pluginRepository;
            }
        }

        public ISiteRepository SiteRepository
        {
            get
            {
                if (_siteRepository == null)
                {
                    _siteRepository = new SiteRepository(_dbConnection, _commands);
                }

                return _siteRepository;
            }
        }

        public void Begin()
        {
            _transactionScope = _dbConnection.BeginTransaction();
        }

        public void RollBack()
        {
            if (_transactionScope == null)
            {
                throw new Exception("Transaction is missing. Please call the Begin method first.");
            }

            _transactionScope.Rollback();
        }

        public void Commit()
        {
            if (_transactionScope == null)
            {
                throw new Exception("Transaction is missing. Please call the Begin method first.");
            }

            _transactionScope.Commit();
        }

        public bool CheckDatabase()
        {
            var dbSetting = _dbConnection.QueryFirstOrDefault<string>("SELECT `Value` FROM SiteSettings WHERE `Key` = @key", new { key = "SYSTEM_INSTALLED" });
            return (dbSetting != null && dbSetting.ToString() == "1");
        }

        public void MarkAsInstalled()
        {
            _dbConnection.Execute("UPDATE SiteSettings SET `Value`='1' WHERE `Key`=@key", new { key = "SYSTEM_INSTALLED" });
        }

        public void Dispose()
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
            _dbConnection = null;
        }
    }
}
