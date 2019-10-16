using Mystique.Core.Configurations;
using Mystique.Core.Consts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Mystique.Core.Helpers
{
    public class AdvancedReferenceLoader : IRefenerceLoader
    {
        private string _moduleFolder = string.Empty;
        private Assembly _assembly = null;
        private string _jsonFilePath = string.Empty;
        private DependanceConfiguration _configuration = new DependanceConfiguration();

        public AdvancedReferenceLoader(string moduleFolder, Assembly assembly, string jsonFilePath)
        {
            _moduleFolder = moduleFolder;
            _assembly = assembly;
            _jsonFilePath = jsonFilePath;

            using (var sr = new StreamReader(_jsonFilePath))
            {
                var content = sr.ReadToEnd();
                _configuration = JsonConvert.DeserializeObject<DependanceConfiguration>(content);
            }
        }

        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context)
        {
            var references = _assembly.GetReferencedAssemblies();

            foreach (var item in references)
            {
                var stream = DefaultReferenceContainer.GetStream(item.Name, item.Version?.ToString());

                if (stream != null)
                {
                    context.LoadFromStream(stream);
                }
                else
                {
                    var name = item.Name;

                    //1.0.0.0 => 1.0.0
                    var version = item.Version.ToString();

                    if (version.Split('.').Length == 4)
                    {
                        version = version.Substring(0, item.Version.ToString().LastIndexOf("."));
                    }

                    if (IsSharedFreamwork(name))
                    {
                        continue;
                    }

                    var key = $"{name}/{version.ToString()}";
                    var obj = _configuration.Targets[".NETCoreApp,Version=v3.0"][key]["compile"].Children().First().Path;

                    //TODO: this will use the Regex later
                    var dllName = obj.Substring(obj.LastIndexOf("[")).Replace("['", string.Empty).Replace("']", string.Empty);

                    using (var sr = new StreamReader($"{_moduleFolder}\\{dllName}"))
                    {
                        var baseStream = sr.BaseStream;

                        var memoryStream = new MemoryStream();
                        baseStream.CopyTo(memoryStream);
                        baseStream.Position = 0;
                        var assembly = context.LoadFromStream(sr.BaseStream);

                        memoryStream.Position = 0;
                        DefaultReferenceContainer.SaveStream(name, version, memoryStream);
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
