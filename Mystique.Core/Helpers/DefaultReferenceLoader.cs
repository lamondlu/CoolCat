using Microsoft.Extensions.Logging;
using Mystique.Core.Configurations;
using Mystique.Core.Consts;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mystique.Core.Helpers
{
    public class DefaultReferenceLoader : IReferenceLoader
    {
        private IReferenceContainer referenceContainer;
        private readonly ILogger<DefaultReferenceLoader> logger;
        private IDependanceLoader dependanceLoader;
        private List<DependanceItem> depandanceItems;

        public DefaultReferenceLoader(IReferenceContainer referenceContainer, ILogger<DefaultReferenceLoader> logger, IDependanceLoader dependanceLoader)
        {
            this.referenceContainer = referenceContainer;
            this.logger = logger;
            this.dependanceLoader = dependanceLoader;
        }

        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context, string moduleFolder, Assembly assembly, string jsonFilePath)
        {
            var references = assembly.GetReferencedAssemblies();

            if (depandanceItems == null)
            {
                depandanceItems = dependanceLoader.GetDependanceItems(jsonFilePath);
            }

            foreach (var item in references)
            {
                var name = item.Name;

                //1.0.0.0 => 1.0.0
                var version = item.Version.ToString();

                //if (version.Split('.').Length == 4)
                //{
                //    version = version.Substring(0, item.Version.ToString().LastIndexOf("."));
                //}

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

                    var package = depandanceItems.SingleOrDefault(p => p.PackageName == name);
                    var dllPath = package.DLLPath;

                    var filePath = Path.Combine(moduleFolder, package.FileName);
                    if (!File.Exists(filePath))
                    {
                        filePath = Path.Combine(moduleFolder, dllPath);
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

                        LoadStreamsIntoContext(context, moduleFolder, referenceAssembly, jsonFilePath);
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