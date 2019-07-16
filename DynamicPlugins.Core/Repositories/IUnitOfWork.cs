using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Repositories
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
