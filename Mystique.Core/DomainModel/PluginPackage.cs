using Mystique.Core.Contracts;
using Mystique.Core.Exceptions;
using Mystique.Core.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ZipTool = System.IO.Compression.ZipArchive;

namespace Mystique.Core.DomainModel
{
    public class PluginPackage
    {
        private string tempFolderName;
        private string folderName;
        private readonly PluginDbContext pluginDbContext;
        private readonly IUnitOfWork unitOfWork;

        private Stream zipStream;

        public PluginConfiguration PluginConfiguration { get; private set; }

        public PluginPackage(PluginDbContext pluginDbContext, IUnitOfWork unitOfWork)
        {
            this.pluginDbContext = pluginDbContext;
            this.unitOfWork = unitOfWork;
        }

        public List<IMigration> GetAllMigrations()
        {
            var context = new CollectibleAssemblyLoadContext();
            var assemblyPath = Path.Combine(tempFolderName, $"{PluginConfiguration.Name}.dll");

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

        public async Task InitializeAsync(Stream zipStream)
        {
            var archive = new ZipTool(this.zipStream = zipStream, ZipArchiveMode.Read);
            zipStream.Position = 0;
            tempFolderName = Path.Combine(Environment.CurrentDirectory, "Mystique_plugins", Guid.NewGuid().ToString());
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
                await LoadConfigurationAsync(s);
            }
        }

        public void SetupFolder()
        {
            var archive = new ZipTool(zipStream, ZipArchiveMode.Read);
            zipStream.Position = 0;
            folderName = Path.Combine(Environment.CurrentDirectory, "Mystique_plugins", PluginConfiguration.Name);
            archive.ExtractToDirectory(folderName, true);

            var folder = new DirectoryInfo(tempFolderName);
            folder.Delete(true);
        }

        private async Task LoadConfigurationAsync(Stream stream)
        {
            using var sr = new StreamReader(stream);
            var content = await sr.ReadToEndAsync();
            PluginConfiguration = JsonConvert.DeserializeObject<PluginConfiguration>(content);

            if (PluginConfiguration == null)
            {
                throw new WrongFormatConfigurationException();
            }
        }
    }
}
