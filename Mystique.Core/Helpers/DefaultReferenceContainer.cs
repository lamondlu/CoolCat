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
        public static Dictionary<CachedReferenceItemKey, Stream> CachedReferences = new Dictionary<CachedReferenceItemKey, Stream>();

        public static bool Exist(string name, string version)
        {
            return CachedReferences.Keys.Any(p => p.ReferenceName == name
                && p.Version == version);
        }

        public static void SaveStream(string name, string version, Stream stream)
        {
            if (Exist(name, version))
            {
                return;
            }

            CachedReferences.Add(new CachedReferenceItemKey { ReferenceName = name, Version = version }, stream);
        }

        public static Stream GetStream(string name, string version)
        {
            var key = CachedReferences.Keys.FirstOrDefault(p => p.ReferenceName == name
                && p.Version == version);

            if (key != null)
            {
                CachedReferences[key].Position = 0;
                return CachedReferences[key];
            }

            return null;
        }
    }
}
