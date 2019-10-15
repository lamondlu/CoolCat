using Mystique.Core.DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Mystique.Core.Helpers
{
    public class DefaultReferenceContainer
    {
        public static Dictionary<ReferenceItem, Stream> References = new Dictionary<ReferenceItem, Stream>();

        public static bool Exist(string name, string version)
        {
            return References.Keys.Any(p => p.ReferenceName == name
                && p.Version == version);
        }

        public static void SaveStream(string name, string version, Stream stream)
        {
            References.Add(new ReferenceItem { ReferenceName = name, Version = version }, stream);
        }

        public static Stream GetStream(string name, string version)
        {
            var key = References.Keys.FirstOrDefault(p => p.ReferenceName == name
                && p.Version == version);

            if (key != null)
            {
                References[key].Position = 0;
                return References[key];
            }

            return null;
        }
    }
}
