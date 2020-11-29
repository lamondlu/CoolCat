using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystique.Core.Contracts
{
    public interface ICollectibleAssemblyLoadContextProvider
    {
        CollectibleAssemblyLoadContext Get(string moduleName);
    }
}
