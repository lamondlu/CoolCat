using Mystique.Core.Helpers;
using Mystique.Core.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Mystique.Core.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly List<Command> commands;
        private readonly string connectionString;
        private readonly DbHelper dbHelper;
        private IPluginRepository pluginRepository;

        public UnitOfWork(IOptions<ConnectionStringSetting> connectionStringAccessor)
        {
            commands = new List<Command>();
            connectionString = connectionStringAccessor.Value.ConnectionString;
            dbHelper = new DbHelper(connectionString);
        }

        public IPluginRepository PluginRepository => pluginRepository ?? (pluginRepository = new PluginRepository(dbHelper, commands));

        public void Commit() => dbHelper.ExecuteNonQuery(commands);
    }
}
