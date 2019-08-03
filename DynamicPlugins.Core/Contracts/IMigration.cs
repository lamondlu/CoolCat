using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Contracts
{
    public interface IMigration
    {
        DomainModel.Version Version { get; }

        void MigrationUp(Guid pluginId);

        void MigrationDown(Guid pluginId);
    }
}
