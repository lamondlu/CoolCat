using System;

namespace CoolCat.Core.Exceptions
{
    public class WrongFormatConfigurationException : Exception
    {
        public WrongFormatConfigurationException() : base("The configuration file is wrong format.")
        {

        }
    }
}
