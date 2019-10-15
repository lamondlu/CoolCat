using Mystique.Core.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mystique.Core.Helpers
{
    public class DefaultReferenceLoader : IRefenerceLoader
    {
        private string _referenceContent = string.Empty;
        private string _folderName = string.Empty;
        private string _excludeFile = string.Empty;

        public DefaultReferenceLoader(string folderName, string excludeFile)
        {
            _folderName = folderName;
            _excludeFile = excludeFile;
        }

        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context)
        {
            var streams = new List<Stream>();
            var di = new DirectoryInfo(_folderName);
            var allReferences = di.GetFiles("*.dll").Where(p => p.Name != _excludeFile);

            foreach (var file in allReferences)
            {
                using (var sr = new StreamReader(file.OpenRead()))
                {
                    context.LoadFromStream(sr.BaseStream);
                }
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
