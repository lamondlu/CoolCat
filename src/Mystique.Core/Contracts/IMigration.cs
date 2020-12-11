using System;

namespace Mystique.Core.Contracts
{
    public interface IMigration
    {
        DomainModel.Version Version { get; }

        void MigrateUp(Guid pluginId);

        void MigrateDown(Guid pluginId);
    }
}
