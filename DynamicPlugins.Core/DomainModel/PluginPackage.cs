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
            var context = new CollectibleAssemblyLoadContext();
            var assemblyPath = $"{_folderName}/{_pluginConfiguration.Name}.dll";

            using (var fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
            {
                var dbHelper = new DbHelper(connectionString);
                var assembly = context.LoadFromStream(fs);
                var migrationTypes = assembly.ExportedTypes.Where(p => p.GetInterfaces().Contains(typeof(IMigration)));

                List<IMigration> migrations = new List<IMigration>();
                foreach (var migrationType in migrationTypes)
                {
                    var constructor = migrationType.GetConstructors().First(p => p.GetParameters().Count() == 1 && p.GetParameters()[0].ParameterType == typeof(DbHelper));

                    migrations.Add((IMigration)constructor.Invoke(new object[] { dbHelper }));
                }

                context.Unload();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                return migrations.OrderBy(p => p.Version).ToList();
            }
        }

        public void Initialize(Stream stream)
        {
            _zipStream = stream;
            var tempFolderName = $"{ AppDomain.CurrentDomain.BaseDirectory }{ Guid.NewGuid().ToString()}";
            ZipTool archive = new ZipTool(_zipStream, ZipArchiveMode.Read);

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
        }

        public void SetupFolder()
        {
            ZipTool archive = new ZipTool(_zipStream, ZipArchiveMode.Read);
            _zipStream.Position = 0;
            _folderName = $"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{_pluginConfiguration.Name}";

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
