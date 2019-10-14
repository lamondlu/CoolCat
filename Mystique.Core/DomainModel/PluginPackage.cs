using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using ZipTool = System.IO.Compression.ZipArchive;
using System.Linq;
using Newtonsoft.Json;
using Mystique.Core.Contracts;
using Mystique.Core.Helpers;

namespace Mystique.Core.DomainModel
{
    public class PluginPackage
    {
        private Stream zipStream = null;
        private string _tempFolderName = string.Empty;
        private string _folderName = string.Empty;

        public PluginConfiguration Configuration { get; private set; }

        public PluginPackage(Stream zipStream)
        {
            this.zipStream = zipStream;
            Initialize(zipStream);
        }

        public List<IMigration> GetAllMigrations(string connectionString)
        {
            var context = new CollectibleAssemblyLoadContext();
            var assemblyPath = $"{_tempFolderName}/{Configuration.Name}.dll";

            using var fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read);
            var dbHelper = new DbHelper(connectionString);
            var assembly = context.LoadFromStream(fs);

            var migrations = assembly.ExportedTypes.Where(p => p.GetInterfaces().Contains(typeof(IMigration))).Select(migrationType =>
            {
                var constructor = migrationType.GetConstructors().First(p => p.GetParameters().Count() == 1 && p.GetParameters()[0].ParameterType == typeof(DbHelper));
                return (IMigration)constructor.Invoke(new object[] { dbHelper });
            }).ToList();

            context.Unload();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return migrations.OrderBy(p => p.Version).ToList();
        }

        public void Initialize(Stream stream)
        {
            zipStream = stream;
            _tempFolderName = $"{ AppDomain.CurrentDomain.BaseDirectory }{ Guid.NewGuid().ToString()}";
            ZipTool archive = new ZipTool(zipStream, ZipArchiveMode.Read);

            archive.ExtractToDirectory(_tempFolderName);

            var folder = new DirectoryInfo(_tempFolderName);

            var files = folder.GetFiles();

            var configFiles = files.Where(p => p.Name == "plugin.json");

            if (!configFiles.Any())
            {
                throw new Exception("The plugin is missing the configuration file.");
            }
            else
            {
                using var s = configFiles.First().OpenRead();
                LoadConfiguration(s);
            }
        }

        public void SetupFolder()
        {
            ZipTool archive = new ZipTool(zipStream, ZipArchiveMode.Read);
            zipStream.Position = 0;
            _folderName = $"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{Configuration.Name}";

            archive.ExtractToDirectory(_folderName, true);

            var folder = new DirectoryInfo(_tempFolderName);
            folder.Delete(true);
        }

        private void LoadConfiguration(Stream stream)
        {
            using var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();
            Configuration = JsonConvert.DeserializeObject<PluginConfiguration>(content);

            if (Configuration == null)
            {
                throw new Exception("The configuration file is wrong format.");
            }
        }
    }
}
