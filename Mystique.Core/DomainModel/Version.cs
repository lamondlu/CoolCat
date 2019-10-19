using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mystique.Core.DomainModel
{
    public class Version : IComparable<Version>
    {
        private readonly int primaryVersion;
        private readonly int secondaryVersion;
        private readonly int minorVersion;

        public Version(string versionNumber)
        {
            var versions = versionNumber?.Split('.');
            if (!int.TryParse(versions?.ElementAtOrDefault(0), out primaryVersion)
                || !int.TryParse(versions?.ElementAtOrDefault(1), out secondaryVersion)
                || !int.TryParse(versions?.ElementAtOrDefault(2), out minorVersion))
            {
                throw new ArgumentException($"The version number '{versionNumber}' is invalid.");
            }
        }

        public int PrimaryVersion => primaryVersion;

        public int SecondaryVersion => secondaryVersion;

        public int MinorVersion => minorVersion;

        public int CompareTo([AllowNull] Version other)
        {
            if (PrimaryVersion > other?.PrimaryVersion
                || (PrimaryVersion == other?.PrimaryVersion && SecondaryVersion > other?.SecondaryVersion)
                || (PrimaryVersion == other?.PrimaryVersion && SecondaryVersion == other?.SecondaryVersion && MinorVersion > other?.MinorVersion))
            {
                return 1;
            }
            else if (PrimaryVersion == other?.PrimaryVersion && SecondaryVersion == other?.SecondaryVersion && MinorVersion == other?.MinorVersion)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public override bool Equals(object obj) => obj is Version version && CompareTo(version) == 0;

        public override int GetHashCode() => HashCode.Combine(PrimaryVersion, SecondaryVersion, MinorVersion);

        public override string ToString() => $"{PrimaryVersion}.{SecondaryVersion}.{MinorVersion}";

        public static bool operator ==(Version left, Version right) => left != null && left.Equals(right);

        public static bool operator !=(Version left, Version right) => !(left == right);

        public static implicit operator Version(string versionNumber) => new Version(versionNumber);

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
