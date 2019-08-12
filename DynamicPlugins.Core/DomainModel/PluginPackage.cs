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
        private Stream _zipStream = null;

        private string _folderName = string.Empty;

        public PluginConfiguration Configuration
        {
            get
            {
                return _pluginConfiguration;
            }
        }

        public PluginPackage(Stream stream)
        {
            _zipStream = stream;
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

            assembly = null;

            return migrations.OrderBy(p => p.Version).ToList();
        }

        public void Initialize(Stream stream)
        {
            var tempFolderName = $"{ AppDomain.CurrentDomain.BaseDirectory }{ Guid.NewGuid().ToString()}";
            ZipTool archive = new ZipTool(stream, ZipArchiveMode.Read);

            archive.ExtractToDirectory(tempFolderName);

            var folder = new DirectoryInfo(tempFolderName);

            var files = folder.GetFiles();

            var configFiles = files.Where(p => p.Name == "plugin.json");

            if (!configFiles.Any())
            {
                throw new Exception("The plugin is missing the configuration file.");
            }
            else
            {
                using (var s = configFiles.First().OpenRead())
                {
                    LoadConfiguration(s);
                }
            }

            folder.Delete(true);

            _folderName = $"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{_pluginConfiguration.Name}";

            if (Directory.Exists(_folderName))
            {
                throw new Exception("The plugin has been existed.");
            }

            stream.Position = 0;
            archive.ExtractToDirectory(_folderName);
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
    }
}
