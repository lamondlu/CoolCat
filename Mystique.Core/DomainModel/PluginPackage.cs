using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using ZipTool = System.IO.Compression.ZipArchive;
using System.Linq;
using Newtonsoft.Json;
using Mystique.Core.Contracts;
using System.Reflection;
using Mystique.Core.Helpers;
using Mystique.Core.Exceptions;

namespace Mystique.Core.DomainModel
{
    public class PluginPackage
    {
        private Stream _zipStream = null;
        private readonly IReferenceLoader referenceLoader;
        private string _tempFolderName = string.Empty;
        private string _folderName = string.Empty;

        public PluginConfiguration Configuration { get; private set; }

        public PluginPackage(IReferenceLoader referenceLoader)
        {
            this.referenceLoader = referenceLoader;
        }

        public List<IMigration> GetAllMigrations(string connectionString)
        {
            var context = new CollectibleAssemblyLoadContext();
            var assemblyPath = Path.Combine(_tempFolderName, $"{Configuration.Name}.dll");

            List<IMigration> migrations = new List<IMigration>();
            using (var fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
            {
                var dbHelper = new DbHelper(connectionString);
                var assembly = context.LoadFromStream(fs);
                referenceLoader.LoadStreamsIntoContext(context, _tempFolderName, assembly);
                var migrationTypes = assembly.ExportedTypes.Where(p => p.GetInterfaces().Contains(typeof(IMigration)));

                foreach (var migrationType in migrationTypes)
                {
                    var constructor = migrationType.GetConstructors().First(p => p.GetParameters().Count() == 1 && p.GetParameters()[0].ParameterType == typeof(DbHelper));

                    migrations.Add((IMigration)constructor.Invoke(new object[] { dbHelper }));
                }
                context.Unload();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return migrations.OrderBy(p => p.Version).ToList();
        }

        public void Initialize(Stream stream)
        {
            _tempFolderName = Path.Combine(Environment.CurrentDirectory, "Mystique_Plugins", Guid.NewGuid().ToString());
            ZipTool archive = new ZipTool(_zipStream = stream, ZipArchiveMode.Read);

            archive.ExtractToDirectory(_tempFolderName);

            var folder = new DirectoryInfo(_tempFolderName);

            var files = folder.GetFiles();

            var configFile = files.SingleOrDefault(p => p.Name == "plugin.json");

            if (configFile == null)
            {
                throw new MissingConfigurationFileException();
            }
            else
            {
                using (var s = configFile.OpenRead())
                {
                    LoadConfiguration(s);
                }
            }
        }

        public void SetupFolder()
        {
            ZipTool archive = new ZipTool(_zipStream, ZipArchiveMode.Read);
            _zipStream.Position = 0;

            _folderName = Path.Combine(Environment.CurrentDirectory, "Mystique_Plugins", Configuration.Name);

            archive.ExtractToDirectory(_folderName, true);

            var folder = new DirectoryInfo(_tempFolderName);
            folder.Delete(true);
        }

        private void LoadConfiguration(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var content = sr.ReadToEnd();
                Configuration = JsonConvert.DeserializeObject<PluginConfiguration>(content);

                if (Configuration == null)
                {
                    throw new WrongFormatConfigurationException();
                }
            }
        }
    }
}
