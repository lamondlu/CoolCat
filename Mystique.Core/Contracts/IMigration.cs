using System;
using System.Threading.Tasks;

namespace Mystique.Core.Contracts
{
    public interface IMigration
    {
        DomainModel.Version Version { get; }

        Task MigrationUpAsync(Guid pluginId);

        Task MigrationDownAsync(Guid pluginId);
    }
}
