using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Repositories
{
    public interface IUnitOfWork
    {
        IPluginRepository PluginRepository { get; }

        void Commit();
    }
}
