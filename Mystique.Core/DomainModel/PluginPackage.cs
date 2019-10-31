using Mystique.Core.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Mystique.Core.DomainModel
{
    public class PluginPackage
    {
        private string tempFolderName;
        private string folderName;
        private Stream zipStream;

        public PluginConfiguration PluginConfiguration { get; private set; }

        public async Task InitializeAsync(Stream zipStream)
        {
            var archive = new ZipArchive(this.zipStream = zipStream, ZipArchiveMode.Read);
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
            var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            zipStream.Position = 0;
            folderName = Path.Combine(Environment.CurrentDirectory, "Mystique_plugins", PluginConfiguration.Name);
            archive.ExtractToDirectory(folderName, true);

            var folder = new DirectoryInfo(tempFolderName);
            if (folder.Exists)
            {
                folder.Delete(true);
            }
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
