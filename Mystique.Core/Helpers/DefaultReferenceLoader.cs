using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mystique.Core.Helpers
{
    public class DefaultReferenceLoader : IRefenerceLoader
    {
        private readonly string folderName;
        private readonly string excludeFile;

        public DefaultReferenceLoader(string folderName, string excludeFile)
        {
            this.folderName = folderName;
            this.excludeFile = excludeFile;
        }

        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context)
        {
            var streams = new List<Stream>();
            var di = new DirectoryInfo(folderName);
            var allReferences = di.GetFiles("*.dll").Where(p => p.Name != excludeFile);

            foreach (var file in allReferences)
            {
                using var sr = new StreamReader(file.OpenRead());
                context.LoadFromStream(sr.BaseStream);
            }
        }

        //public List<string> GetReferences()
        //{
        //    var content = _dependanceConfiguration.Libraries;

        //    foreach (var item in content.Descendants().ToList())
        //    {
        //        var itemKey = item.Path;
        //        var type = item.Children().First().Values<DependanceConfiguration>();

        //        var a = item.Children().First().Children().f().Values<string>();
        //    }

        //    return new List<string>();
        //}
    }
}
