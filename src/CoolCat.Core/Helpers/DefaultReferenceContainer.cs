using CoolCat.Core.Contracts;
using CoolCat.Core.DomainModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoolCat.Core.Helpers
{
    public class DefaultReferenceContainer : IReferenceContainer
    {
        private static readonly Dictionary<CachedReferenceItemKey, Stream> _cachedReferences = new Dictionary<CachedReferenceItemKey, Stream>();


        public List<CachedReferenceItemKey> GetAll()
        {
            return _cachedReferences.Keys.ToList();
        }

        public bool Exist(string name, string version)
        {
            return _cachedReferences.Keys.Any(p => p.ReferenceName == name
                && p.Version == version);
        }

        public void SaveStream(string name, string version, Stream stream)
        {
            if (Exist(name, version))
            {
                return;
            }


            _cachedReferences.Add(new CachedReferenceItemKey { ReferenceName = name, Version = version }, stream);
        }

        public Stream GetStream(string name, string version)
        {
            CachedReferenceItemKey key = _cachedReferences.Keys.FirstOrDefault(p => p.ReferenceName == name
                && p.Version == version);

            if (key != null)
            {
                _cachedReferences[key].Position = 0;
                return _cachedReferences[key];
            }

            return null;
        }
    }
}
