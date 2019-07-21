using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;

namespace ZipTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var pluginName = Guid.NewGuid().ToString();

            using (var fs = new FileStream("DemoPlugin1.zip", FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(pluginName);
                }
            }

            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + pluginName);

            var files = directory.GetFiles();

            foreach (var file in files)
            {
                Console.WriteLine($"{file.Name}");

                if (file.Name == "plugin.json")
                {
                    var fs = new StreamReader(file.OpenRead());

                    var content = fs.ReadToEnd();

                    var obj = JsonConvert.DeserializeObject<PluginConfiguration>(content);
                    Console.WriteLine(JsonConvert.SerializeObject(obj));

                }
            }

            Console.Read();
        }
    }
}
