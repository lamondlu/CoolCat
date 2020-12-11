using Microsoft.Extensions.Logging;
using Mystique.Core.Consts;
using Mystique.Core.Contracts;
using System.IO;
using System.Reflection;

namespace Mystique.Core.Helpers
{
    public class DefaultReferenceLoader : IReferenceLoader
    {
        private readonly IReferenceContainer _referenceContainer = null;
        private readonly ILogger<DefaultReferenceLoader> _logger = null;

        public DefaultReferenceLoader(IReferenceContainer referenceContainer, ILogger<DefaultReferenceLoader> logger)
        {
            _referenceContainer = referenceContainer;
            _logger = logger;
        }

        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context, string moduleFolder, Assembly assembly)
        {
            AssemblyName[] references = assembly.GetReferencedAssemblies();

            foreach (AssemblyName item in references)
            {
                string name = item.Name;

                string version = item.Version.ToString();

                Stream stream = _referenceContainer.GetStream(name, version);

                if (stream != null)
                {
                    _logger.LogDebug($"Found the cached reference '{name}' v.{version}");
                    context.LoadFromStream(stream);
                }
                else
                {

                    if (IsSharedFreamwork(name) || name.Contains("Mystique.Core"))
                    {
                        continue;
                    }

                    string dllName = $"{name}.dll";
                    string filePath = $"{moduleFolder}/{dllName}";

                    if (!File.Exists(filePath))
                    {
                        _logger.LogWarning($"The package '{dllName}' in '{filePath}' is missing.");
                        continue;
                    }

                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        Assembly referenceAssembly = context.LoadFromStream(fs);

                        MemoryStream memoryStream = new MemoryStream();

                        fs.Position = 0;
                        fs.CopyTo(memoryStream);
                        fs.Position = 0;
                        memoryStream.Position = 0;
                        _referenceContainer.SaveStream(name, version, memoryStream);

                        LoadStreamsIntoContext(context, moduleFolder, referenceAssembly);
                    }
                }
            }
        }

        private bool IsSharedFreamwork(string name)
        {
            return SharedFrameworkConst.SharedFrameworkDLLs.Contains($"{name}.dll");
        }
    }
}
