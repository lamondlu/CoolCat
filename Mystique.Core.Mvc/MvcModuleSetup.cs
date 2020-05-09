using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Mystique.Core.Contracts;
using Mystique.Core.Mvc.Infrastructure;
using Mystique.Mvc.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace Mystique.Core.Mvc
{
    public class MvcModuleSetup : IMvcModuleSetup
    {
        private readonly ApplicationPartManager _partManager;
        private readonly IReferenceLoader _referenceLoader = null;

        public MvcModuleSetup(ApplicationPartManager partManager, IReferenceLoader referenceLoader)
        {
            _partManager = partManager;
            _referenceLoader = referenceLoader;
        }

        public void EnableModule(string moduleName)
        {
            if (!PluginsLoadContexts.Any(moduleName))
            {
                CollectibleAssemblyLoadContext context = new CollectibleAssemblyLoadContext();

                string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{moduleName}\\{moduleName}.dll";
                string referenceFolderPath = $"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{moduleName}";
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    System.Reflection.Assembly assembly = context.LoadFromStream(fs);
                    _referenceLoader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                    MystiqueAssemblyPart controllerAssemblyPart = new MystiqueAssemblyPart(assembly);

                    AdditionalReferencePathHolder.AdditionalReferencePaths.Add(filePath);
                    _partManager.ApplicationParts.Add(controllerAssemblyPart);
                    PluginsLoadContexts.Add(moduleName, context);
                }
            }
            else
            {
                CollectibleAssemblyLoadContext context = PluginsLoadContexts.Get(moduleName);
                MystiqueAssemblyPart controllerAssemblyPart = new MystiqueAssemblyPart(context.Assemblies.First());
                _partManager.ApplicationParts.Add(controllerAssemblyPart);
            }

            ResetControllActions();
        }

        public void DisableModule(string moduleName)
        {
            ApplicationPart last = _partManager.ApplicationParts.First(p => p.Name == moduleName);
            _partManager.ApplicationParts.Remove(last);

            ResetControllActions();
        }

        public void DeleteModule(string moduleName)
        {
            PluginsLoadContexts.Remove(moduleName);

            DirectoryInfo directory = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{moduleName}");
            directory.Delete(true);
        }

        private void ResetControllActions()
        {
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }
    }
}
