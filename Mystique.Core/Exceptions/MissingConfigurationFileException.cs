using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Exceptions
{
    public class MissingConfigurationFileException : Exception
    {
        public MissingConfigurationFileException() : base("The plugin is missing the configuration file.")
        {

        }
    }
}
