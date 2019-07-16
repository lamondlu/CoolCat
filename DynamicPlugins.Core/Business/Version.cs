using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.Business
{
    public class Version
    {
        public string VersionNumber { get; set; }

        public static bool operator ==(Version left, Version right)
        {
            if (left == null || right == null)
            {
                return false;
            }

            return left.VersionNumber.Equals(right.VersionNumber);
        }

        public static bool operator !=(Version left, Version right)
        {
            return !(left == right);
        }
    }
}
