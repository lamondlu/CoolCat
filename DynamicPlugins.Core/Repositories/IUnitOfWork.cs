using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Repositories
{
    public interface IUnitOfWork
    {
        IPluginRepository PluginRepository { get; }

        void Commit();
    }
}
