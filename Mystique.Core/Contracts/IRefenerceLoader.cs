using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Mystique.Core.Contracts
{
    public interface IReferenceLoader
    {
        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context, string moduleFolder, Assembly assembly, string jsonFilePath);
    }
}
