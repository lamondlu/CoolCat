using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Contracts
{
    public interface IMigration
    {
        Version Version { get; }

        void Up();

        void Down();
    }
}
