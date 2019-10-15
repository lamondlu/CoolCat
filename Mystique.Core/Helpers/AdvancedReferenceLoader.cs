using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mystique.Core.Helpers
{
    public class AdvancedReferenceLoader : IRefenerceLoader
    {
        private string _jsonFile = string.Empty;
        private Assembly _assembly = null;

        public AdvancedReferenceLoader(string jsonFile, Assembly assembly)
        {
            _jsonFile = jsonFile;
            _assembly = assembly;
        }

        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context)
        {
            var references = _assembly.GetReferencedAssemblies();

            foreach (var item in references)
            {
                Console.WriteLine(item.FullName);
            }
        }
    }
}
