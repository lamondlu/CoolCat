using System;

namespace Mystique.Core.Exceptions
{
    public class MissingConfigurationFileException : Exception
    {
        public MissingConfigurationFileException() : base("The plugin is missing the configuration file.")
        {

        }
    }
}
