using Mystique.Core.Configurations;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Mystique.Core.Helpers
{
    public class DefaultDependanceLoader : IDependanceLoader
    {
        public List<DependanceItem> GetDependanceItems(string jsonFilePath)
        {
            var items = new List<DependanceItem>();

            using (var sr = new StreamReader(jsonFilePath))
            {
                var content = sr.ReadToEnd();
                var configuration = JsonConvert.DeserializeObject<DependanceConfiguration>(content);

                var targetsPart = configuration.Targets;

                foreach (var item in targetsPart[".NETCoreApp,Version=v3.0"])
                {
                    var key = item.Path.Replace("['.NETCoreApp,Version=v3.0']", string.Empty);
                    key = key.Replace("['", string.Empty);
                    key = key.Replace("']", string.Empty);

                    var name = key.Split('/')[0];
                    var version = key.Split('/')[1];
                    var assemblyVersion = string.Empty;

                    var node = targetsPart[".NETCoreApp,Version=v3.0"][$"{name}/{version}"]?.ToString() ?? string.Empty;
                    var runtime = targetsPart[".NETCoreApp,Version=v3.0"][$"{name}/{version}"]["runtime"];

                    if (string.IsNullOrWhiteSpace(node) || node == "{}" || runtime == null)
                    {
                        continue;
                    }
                    else
                    {
                        var pathNode = runtime.Children().First().Path;
                        var path = pathNode.Replace($"['.NETCoreApp,Version=v3.0']['{key}'].runtime", string.Empty);
                        path = path.Replace("['", string.Empty);
                        path = path.Replace("']", string.Empty);

                        var dependanceItem = new DependanceItem(name, version, assemblyVersion, path);
                        items.Add(dependanceItem);
                    }
                }
            }

            return items;
        }
    }
}
