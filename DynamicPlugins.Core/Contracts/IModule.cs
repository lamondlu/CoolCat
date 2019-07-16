using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Contracts
{
    public interface IModule
    {
        string Name { get; }

        Business.Version Version { get; set; }
    }
}
