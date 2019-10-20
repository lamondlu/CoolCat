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
        private IReferenceContainer _referenceContainer = null;
        private readonly ILogger<DefaultReferenceLoader> _logger = null;
        private IDependanceLoader _dependanceLoader = null;
        private List<DependanceItem> _depandanceItems = null;

        public DefaultReferenceLoader(IReferenceContainer referenceContainer, ILogger<DefaultReferenceLoader> logger, IDependanceLoader dependanceLoader)
        {
            _referenceContainer = referenceContainer;
            _logger = logger;
            _dependanceLoader = dependanceLoader;
        }

        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context, string moduleFolder, Assembly assembly, string jsonFilePath)
        {
            var references = assembly.GetReferencedAssemblies();

            if (_depandanceItems == null)
            {
                _depandanceItems = _dependanceLoader.GetDependanceItems(jsonFilePath);
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

                var stream = _referenceContainer.GetStream(name, version);

                if (stream != null)
                {
                    _logger.LogDebug($"Found the cached reference '{name}' v.{version}");
                    context.LoadFromStream(stream);
                }
                else
                {

                    if (IsSharedFreamwork(name))
                    {
                        continue;
                    }

                    var package = _depandanceItems.SingleOrDefault(p => p.PackageName == name);
                    var dllPath = package.DLLPath;

                    var filePath = $"{moduleFolder}\\{package.FileName}";
                    if (!File.Exists(filePath))
                    {
                        filePath = $"{moduleFolder}\\{dllPath}";
                    }

                    filePath = filePath.Replace("/", "\\");

                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var referenceAssembly = context.LoadFromStream(fs);

                        var memoryStream = new MemoryStream();

                        fs.Position = 0;
                        fs.CopyTo(memoryStream);
                        fs.Position = 0;
                        memoryStream.Position = 0;
                        _referenceContainer.SaveStream(name, version, memoryStream);

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
