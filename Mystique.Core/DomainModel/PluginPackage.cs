using Mystique.Core.Contracts;
using Mystique.Core.Exceptions;
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

        public PluginConfiguration PluginConfiguration { get; private set; }

        public PluginPackage(PluginDbContext pluginDbContext, IUnitOfWork unitOfWork)
        {
            this.pluginDbContext = pluginDbContext;
            this.unitOfWork = unitOfWork;
        }

        public List<IMigration> GetAllMigrations()
        {
            var context = new CollectibleAssemblyLoadContext();
            var assemblyPath = $"{tempFolderName}/{PluginConfiguration.Name}.dll";

            using var fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read);
            var assembly = context.LoadFromStream(fs);

            var migrations = assembly.ExportedTypes.Where(p => p.GetInterfaces().Contains(typeof(IMigration))).Select(migrationType =>
            {
                return Activator.CreateInstance(migrationType, new object[] { pluginDbContext, unitOfWork }) as IMigration;
            }).ToList();

            context.Unload();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return migrations.OrderBy(p => p.Version).ToList();
        }

        public void Initialize(Stream stream)
        {
            zipStream = stream;
            tempFolderName = $"{AppDomain.CurrentDomain.BaseDirectory}{Guid.NewGuid().ToString()}";
            ZipTool archive = new ZipTool(zipStream, ZipArchiveMode.Read);

            archive.ExtractToDirectory(tempFolderName);

            var folder = new DirectoryInfo(tempFolderName);
            var files = folder.GetFiles();
            var configFile = files.SingleOrDefault(p => p.Name == "plugin.json");

            if (configFile == null)
            {
                throw new MissingConfigurationFileException();
            }
            else
            {
                using var s = configFile.OpenRead();
                LoadConfiguration(s);
            }
        }

        public void SetupFolder()
        {
            ZipTool archive = new ZipTool(zipStream, ZipArchiveMode.Read);
            zipStream.Position = 0;
            folderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", PluginConfiguration.Name);

            archive.ExtractToDirectory(folderName, true);

            var folder = new DirectoryInfo(tempFolderName);
            folder.Delete(true);
        }

        private void LoadConfiguration(Stream stream)
        {
            using var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();
            PluginConfiguration = JsonConvert.DeserializeObject<PluginConfiguration>(content);

            if (PluginConfiguration == null)
            {
                throw new WrongFormatConfigurationException();
            }
        }
    }
}
