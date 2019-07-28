using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Contracts
{
    public interface IMigration
    {
        string Version { get; }

        void Up();

        void Down();
    }
}
