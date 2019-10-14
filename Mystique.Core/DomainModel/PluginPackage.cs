using Mystique.Core.Contracts;
using Mystique.Core.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ZipTool = System.IO.Compression.ZipArchive;

namespace Mystique.Core.DomainModel
{
    public class PluginPackage
    {
        private Stream zipStream;
        private string tempFolderName;
        private string folderName;
        private readonly PluginDbContext pluginDbContext;
        private readonly IUnitOfWork unitOfWork;

        public PluginConfiguration Configuration { get; private set; }

        public PluginPackage(PluginDbContext pluginDbContext, IUnitOfWork unitOfWork)
        {
            this.pluginDbContext = pluginDbContext;
            this.unitOfWork = unitOfWork;
        }

        public void Initialize(Stream stream)
        {
            zipStream = stream;
            tempFolderName = $"{AppDomain.CurrentDomain.BaseDirectory}{Guid.NewGuid().ToString()}";
            ZipTool archive = new ZipTool(zipStream, ZipArchiveMode.Read);

            archive.ExtractToDirectory(tempFolderName);

            var folder = new DirectoryInfo(tempFolderName);
            var files = folder.GetFiles();
            var configFiles = files.Where(p => p.Name == "plugin.json");
            if (!configFiles.Any())
            {
                throw new Exception("The plugin is missing the configuration file.");
            }
            using var s = configFiles.First().OpenRead();
            LoadConfiguration(s);
        }

        public List<IMigration> GetAllMigrations()
        {
            var context = new CollectibleAssemblyLoadContext();
            var assemblyPath = $"{tempFolderName}/{Configuration.Name}.dll";

            using var fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read);
            var assembly = context.LoadFromStream(fs);

            //var dbHelper = new DbHelper(connectionString);
            //var migrations = assembly.ExportedTypes.Where(p => p.GetInterfaces().Contains(typeof(IMigration))).Select(migrationType =>
            //{
            //    var constructor = migrationType.GetConstructors().First(p => p.GetParameters().Count() == 1 && p.GetParameters()[0].ParameterType == typeof(DbHelper));
            //    return (IMigration)constructor.Invoke(new object[] { dbHelper });
            //}).ToList();

            context.Unload();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return new List<IMigration>();
            // return migrations.OrderBy(p => p.Version).ToList();
        }

        public void SetupFolder()
        {
            ZipTool archive = new ZipTool(zipStream, ZipArchiveMode.Read);
            zipStream.Position = 0;
            folderName = $"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{Configuration.Name}";

            archive.ExtractToDirectory(folderName, true);

            var folder = new DirectoryInfo(tempFolderName);
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
