using System;

namespace Mystique.Core.Contracts
{
    public interface IMigration
    {
        DomainModel.Version Version { get; }

        void MigrationUp(Guid pluginId);

        void MigrationDown(Guid pluginId);
    }
}
