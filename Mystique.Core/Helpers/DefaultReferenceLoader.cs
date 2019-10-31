using Microsoft.Extensions.Logging;
using Mystique.Core.Consts;
using Mystique.Core.Contracts;
using System.IO;
using System.Reflection;

namespace Mystique.Core.Helpers
{
    public class DefaultReferenceLoader : IReferenceLoader
    {
        private IReferenceContainer referenceContainer;
        private readonly ILogger<DefaultReferenceLoader> logger;

        public DefaultReferenceLoader(IReferenceContainer referenceContainer, ILogger<DefaultReferenceLoader> logger)
        {
            this.referenceContainer = referenceContainer;
            this.logger = logger;
        }

        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context, string moduleFolder, Assembly assembly)
        {
            var references = assembly.GetReferencedAssemblies();

            foreach (var item in references)
            {
                var name = item.Name;

                var version = item.Version.ToString();

                var stream = referenceContainer.GetStream(name, version);

                if (stream != null)
                {
                    logger.LogDebug($"Found the cached reference '{name}' v.{version}");
                    context.LoadFromStream(stream);
                }
                else
                {

                    if (IsSharedFreamwork(name))
                    {
                        continue;
                    }

                    var dllName = $"{name}.dll";
                    var filePath = $"{moduleFolder}\\{dllName}";

                    if (!File.Exists(filePath))
                    {
                        logger.LogWarning($"The package '{dllName}' is missing.");
                        continue;
                    }

                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var referenceAssembly = context.LoadFromStream(fs);

                        var memoryStream = new MemoryStream();

                        fs.Position = 0;
                        fs.CopyTo(memoryStream);
                        fs.Position = 0;
                        memoryStream.Position = 0;
                        referenceContainer.SaveStream(name, version, memoryStream);

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