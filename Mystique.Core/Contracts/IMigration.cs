using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Contracts
{
    public interface IMigration
    {
        DomainModel.Version Version { get; }

        void MigrateUp(Guid pluginId);

        void MigrateDown(Guid pluginId);
    }
}
