using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using ZipTool = System.IO.Compression.ZipArchive;
using System.Linq;
using Newtonsoft.Json;

namespace DynamicPlugins.Core.DomainModel
{
    public class ZipFile
    {
        private bool _isValid = false;
        private PluginConfiguration _pluginConfiguration = null;
        private string _folderName = $"{AppDomain.CurrentDomain.BaseDirectory}{Guid.NewGuid().ToString()}";

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
        }

        public ZipFile(Stream stream)
        {
            Initialize(stream);
        }

        public void Initialize(Stream stream)
        {
            using (ZipTool archive = new ZipTool(stream, ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(_folderName);

                var folder = new DirectoryInfo(_folderName);

                var files = folder.GetFiles();

                var configFiles = files.Where(p => p.Name == "plugin.json");

                if (!configFiles.Any())
                {
                    throw new Exception("The plugin is missing the configuration file.");
                }
                else
                {
                    var pluginName = string.Empty;

                    using (var sr = new StreamReader(configFiles.First().OpenRead()))
                    {
                        var content = sr.ReadToEnd();
                        var configuration = JsonConvert.DeserializeObject<PluginConfiguration>(content);

                        if (configuration == null)
                        {
                            throw new Exception("The configuration file is wrong format.");
                        }

                        pluginName = configuration.Name;
                    }


                }
            }
        }
    }
}
