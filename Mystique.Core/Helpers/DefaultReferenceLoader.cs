using Mystique.Core.Configurations;
using Mystique.Core.Consts;
using Mystique.Core.Contracts;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mystique.Core.Helpers
{
    public class DefaultReferenceLoader : IReferenceLoader
    {
        private IReferenceContainer _referenceContainer = null;

        public DefaultReferenceLoader(IReferenceContainer referenceContainer)
        {
            _referenceContainer = referenceContainer;
        }

        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context, string moduleFolder, Assembly assembly, string jsonFilePath)
        {
            var references = assembly.GetReferencedAssemblies();
            DependanceConfiguration configuration = null;

            using (var sr = new StreamReader(jsonFilePath))
            {
                var content = sr.ReadToEnd();
                configuration = JsonConvert.DeserializeObject<DependanceConfiguration>(content);
            }

            foreach (var item in references)
            {
                var stream = _referenceContainer.GetStream(item.Name, item.Version?.ToString());

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
                    var obj = configuration.Targets[".NETCoreApp,Version=v3.0"][key]["compile"].Children().First().Path;

                    //TODO: this will use the Regex later
                    var dllName = obj.Substring(obj.LastIndexOf("[")).Replace("['", string.Empty).Replace("']", string.Empty);

                    using (var sr = new StreamReader($"{moduleFolder}\\{dllName}"))
                    {
                        var baseStream = sr.BaseStream;
                        var referenceAssembly = context.LoadFromStream(baseStream);

                        var memoryStream = new MemoryStream();
                        baseStream.CopyTo(memoryStream);
                        baseStream.Position = 0;
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
