using System.Linq;

namespace Mystique.Core.DomainModel
{
    public class DependanceItem
    {
        public DependanceItem(string packageName, string version, string assemblyVersion, string dllPath)
        {
            PackageName = packageName;
            Version = string.IsNullOrEmpty(assemblyVersion) ? version : assemblyVersion;
            DLLPath = dllPath;
        }

        public string PackageName { get; private set; }

        public string Version { get; private set; }

        public string DLLPath { get; private set; }

        public string FileName => DLLPath.Split('/').Last();
    }
}
