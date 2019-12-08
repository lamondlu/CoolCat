using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Mystique.Core.DomainModel
{
    public class Version : IComparable<Version>
    {
        private const string _pattern = "^[0-9]*$";
        private static Regex _regex = new Regex(_pattern);

        public Version(string versionNumber)
        {
            if (Validate(versionNumber))
            {
                this.VersionNumber = versionNumber;
            }
            else
            {
                throw new ArgumentException("The version number is invalid.");
            }

        }

        public int PrimaryVersion
        {
            get
            {
                return Convert.ToInt32(this.VersionNumber.Split('.')[0]);
            }
        }

        public int SecondaryVersion
        {
            get
            {
                return Convert.ToInt32(this.VersionNumber.Split('.')[1]);
            }
        }

        public int MinorVersion
        {
            get
            {
                return Convert.ToInt32(this.VersionNumber.Split('.')[2]);
            }
        }

        private bool Validate(string versionNumber)
        {
            if (!string.IsNullOrEmpty(versionNumber) && versionNumber.Split(".").Length == 3)
            {
                var primary = versionNumber.Split('.')[0];
                var secondray = versionNumber.Split('.')[1];
                var minor = versionNumber.Split('.')[2];

                return _regex.IsMatch(primary)
                    && _regex.IsMatch(secondray)
                    && _regex.IsMatch(minor);
            }
            else
            {
                return false;
            }
        }

        public string VersionNumber { get; set; }

        public int CompareTo([AllowNull] Version other)
        {
            if (PrimaryVersion > other.PrimaryVersion
                || (PrimaryVersion == other.PrimaryVersion && SecondaryVersion > other.SecondaryVersion)
                || (PrimaryVersion == other.PrimaryVersion && SecondaryVersion == other.SecondaryVersion && MinorVersion > other.MinorVersion))
            {
                return 1;
            }
            else if (PrimaryVersion == other.PrimaryVersion
                && SecondaryVersion == other.SecondaryVersion
                && MinorVersion == other.MinorVersion)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

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

        public static implicit operator Version(string versionNumber)
        {
            return new Version(versionNumber);
        }

        public static bool operator >(Version x, Version y)
        {
            return x.PrimaryVersion > y.PrimaryVersion ||
                (x.PrimaryVersion == y.PrimaryVersion && x.SecondaryVersion > y.SecondaryVersion)
                || (x.PrimaryVersion == y.PrimaryVersion && x.SecondaryVersion == y.SecondaryVersion && x.MinorVersion > y.MinorVersion);
        }

        public static bool operator <(Version x, Version y)
        {
            return x.PrimaryVersion < y.PrimaryVersion ||
               (x.PrimaryVersion == y.PrimaryVersion && x.SecondaryVersion < y.SecondaryVersion)
               || (x.PrimaryVersion == y.PrimaryVersion && x.SecondaryVersion == y.SecondaryVersion && x.MinorVersion < y.MinorVersion);
        }
    }
}
