using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Exceptions
{
    public class WrongFormatConfigurationException : Exception
    {
        public WrongFormatConfigurationException() : base("The configuration file is wrong format.")
        {

        }
    }
}
