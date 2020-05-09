using Mystique.Core.Configurations;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mystique.Core.Helpers
{
    public class DefaultDependanceLoader : IDependanceLoader
    {
        public List<DependanceItem> GetDependanceItems(string jsonFilePath)
        {
            List<DependanceItem> items = new List<DependanceItem>();

            using (StreamReader sr = new StreamReader(jsonFilePath))
            {
                string content = sr.ReadToEnd();
                DependanceConfiguration configuration = JsonConvert.DeserializeObject<DependanceConfiguration>(content);

                JObject targetsPart = configuration.Targets;

                foreach (JToken item in targetsPart[".NETCoreApp,Version=v3.0"])
                {
                    string key = item.Path.Replace("['.NETCoreApp,Version=v3.0']", string.Empty);
                    key = key.Replace("['", string.Empty);
                    key = key.Replace("']", string.Empty);

                    string name = key.Split('/')[0];
                    string version = key.Split('/')[1];
                    string assemblyVersion = string.Empty;

                    string node = targetsPart[".NETCoreApp,Version=v3.0"][$"{name}/{version}"]?.ToString() ?? string.Empty;
                    JToken runtime = targetsPart[".NETCoreApp,Version=v3.0"][$"{name}/{version}"]["runtime"];

                    if (string.IsNullOrWhiteSpace(node) || node == "{}" || runtime == null)
                    {
                        continue;
                    }
                    else
                    {
                        string pathNode = runtime.Children().First().Path;
                        string path = pathNode.Replace($"['.NETCoreApp,Version=v3.0']['{key}'].runtime", string.Empty);
                        path = path.Replace("['", string.Empty);
                        path = path.Replace("']", string.Empty);

                        DependanceItem dependanceItem = new DependanceItem(name, version, assemblyVersion, path);
                        items.Add(dependanceItem);
                    }
                }
            }

            return items;
        }
    }
}
