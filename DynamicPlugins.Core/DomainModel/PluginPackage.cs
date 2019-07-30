using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using ZipTool = System.IO.Compression.ZipArchive;
using System.Linq;
using Newtonsoft.Json;
using DynamicPlugins.Core.Contracts;
using System.Reflection;
using DynamicPlugins.Core.Helpers;

namespace DynamicPlugins.Core.DomainModel
{
    public class PluginPackage
    {
        private PluginConfiguration _pluginConfiguration = null;

        private string _folderName = $"{AppDomain.CurrentDomain.BaseDirectory}{Guid.NewGuid().ToString()}";

        public PluginConfiguration Configuration
        {
            get
            {
                return _pluginConfiguration;
            }
        }

        public PluginPackage(Stream stream)
        {
            Initialize(stream);
        }

        public List<IMigration> GetAllMigrations(string connectionString)
        {
            var assembly = Assembly.LoadFile($"{_folderName}/{_pluginConfiguration.Name}.dll");

            var dbHelper = new DbHelper(connectionString);

            var migrationTypes = assembly.ExportedTypes.Where(p => p.GetInterfaces().Contains(typeof(IMigration)));

            List<IMigration> migrations = new List<IMigration>();
            foreach (var migrationType in migrationTypes)
            {
                var constructor = migrationType.GetConstructors().First(p => p.GetParameters().Count() == 1 && p.GetParameters()[0].ParameterType == typeof(DbHelper));

                migrations.Add((IMigration)constructor.Invoke(new object[] { dbHelper }));
            }

            return migrations;
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
                    LoadConfiguration(configFiles.First().OpenRead());
                }
            }
        }

        private void LoadConfiguration(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var content = sr.ReadToEnd();
                _pluginConfiguration = JsonConvert.DeserializeObject<PluginConfiguration>(content);

                if (_pluginConfiguration == null)
                {
                    throw new Exception("The configuration file is wrong format.");
                }
            }
        }


        public void Save()
        {
            var pluginName = _pluginConfiguration.Name;

            if (!string.IsNullOrEmpty(pluginName))
            {
                var folder = new DirectoryInfo(_folderName);
                var newName = $"{AppDomain.CurrentDomain.BaseDirectory}/Modules/{pluginName}";

                if (!Directory.Exists(newName))
                {
                    folder.MoveTo(newName);
                }
                else
                {
                    throw new Exception("The plugin has been existed.");
                }
            }
        }
    }
}
