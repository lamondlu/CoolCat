using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mystique.Utilities
{
    public class PresetPluginLoader
    {
        public List<string> LoadPlugins()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PresetModules"));

            FileInfo[] files = di.GetFiles("*.zip");

            return files.Select(p => p.Name).ToList();
        }
    }
}
