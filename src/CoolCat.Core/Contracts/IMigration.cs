using System;

namespace CoolCat.Core.Contracts
{
    public interface IMigration
    {
        DomainModel.Version Version { get; }

        void MigrateUp(Guid pluginId);

        void MigrateDown(Guid pluginId);
    }
}
