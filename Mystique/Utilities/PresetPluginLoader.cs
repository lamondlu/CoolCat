using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mystique.Utilities
{
    public class PresetPluginLoader
    {
        public List<string> LoadPlugins()
        {
            var di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PresetModules"));

            var files = di.GetFiles("*.zip");

            return files.Select(p => p.Name).ToList();
        }
    }
}
