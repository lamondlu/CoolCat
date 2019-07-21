using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using ZipTool = System.IO.Compression.ZipArchive;

namespace DynamicPlugins.Core.DomainModel
{
    public class ZipArchive
    {
        private bool _isValid = false;
        private PluginConfiguration _pluginConfiguration = null;

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
        }

        public ZipArchive(Stream stream)
        {
            //using (ZipTool archive = new ZipTool(stream, ZipArchiveMode.Read))
            //{
            //    archive.Entries();
            //}
        }

        public void Initialize(Stream stream)
        {

        }
    }
}
