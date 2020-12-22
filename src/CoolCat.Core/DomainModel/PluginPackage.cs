using CoolCat.Core.Contracts;
using CoolCat.Core.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ZipTool = System.IO.Compression.ZipArchive;

namespace CoolCat.Core.DomainModel
{
    public class PluginPackage
    {
        private PluginConfiguration _pluginConfiguration = null;
        private Stream _zipStream = null;
        private string _tempFolderName = string.Empty;
        private string _folderName = string.Empty;
        private IDbHelper _dbHelper = null;

        public PluginConfiguration Configuration => _pluginConfiguration;

        public PluginPackage(Stream stream, IDbHelper dbHelper)
        {
            _zipStream = stream;
            _dbHelper = dbHelper;
            Initialize(stream);
        }

        public List<IMigration> GetAllMigrations(string connectionString)
        {
            CollectibleAssemblyLoadContext context = new CollectibleAssemblyLoadContext(_pluginConfiguration.Name);
            string assemblyPath = Path.Combine(_tempFolderName, $"{_pluginConfiguration.Name}.dll");

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
            {

                System.Reflection.Assembly assembly = context.LoadFromStream(fs);
                IEnumerable<Type> migrationTypes = assembly.ExportedTypes.Where(p => p.GetInterfaces().Contains(typeof(IMigration)));

                List<IMigration> migrations = new List<IMigration>();
                foreach (Type migrationType in migrationTypes)
                {
                    System.Reflection.ConstructorInfo constructor = migrationType.GetConstructors().First(p => p.GetParameters().Count() == 1 && p.GetParameters()[0].ParameterType == typeof(IDbHelper));

                    migrations.Add((IMigration)constructor.Invoke(new object[] { _dbHelper }));
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
            _tempFolderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{ Guid.NewGuid()}");
            ZipTool archive = new ZipTool(_zipStream, ZipArchiveMode.Read);

            archive.ExtractToDirectory(_tempFolderName);

            DirectoryInfo folder = new DirectoryInfo(_tempFolderName);

            FileInfo[] files = folder.GetFiles();

            FileInfo configFile = files.SingleOrDefault(p => p.Name == "plugin.json");

            if (configFile == null)
            {
                throw new MissingConfigurationFileException();
            }
            else
            {
                using (FileStream s = configFile.OpenRead())
                {
                    LoadConfiguration(s);
                }
            }
        }

        public void SetupFolder()
        {
            ZipTool archive = new ZipTool(_zipStream, ZipArchiveMode.Read);
            _zipStream.Position = 0;
            _folderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", $"{_pluginConfiguration.Name}");

            archive.ExtractToDirectory(_folderName, true);

            DirectoryInfo folder = new DirectoryInfo(_tempFolderName);
            folder.Delete(true);
        }

        private void LoadConfiguration(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                string content = sr.ReadToEnd();
                _pluginConfiguration = JsonConvert.DeserializeObject<PluginConfiguration>(content);

                if (_pluginConfiguration == null)
                {
                    throw new WrongFormatConfigurationException();
                }
            }
        }
    }
}
