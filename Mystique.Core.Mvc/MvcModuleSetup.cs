﻿using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Mystique.Core.Contracts;
using Mystique.Core.Helpers;
using Mystique.Core.Mvc.Infrastructure;
using Mystique.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mystique.Core.Mvc
{
    public class MvcModuleSetup : IMvcModuleSetup
    {
        private ApplicationPartManager _partManager;
        private IReferenceLoader _referenceLoader = null;

        public MvcModuleSetup(ApplicationPartManager partManager, IReferenceLoader referenceLoader)
        {
            _partManager = partManager;
            _referenceLoader = referenceLoader;
        }

    public void EnableModule(string moduleName)
    {
        if (!PluginsLoadContexts.Any(moduleName))
        {
            var context = new CollectibleAssemblyLoadContext();

                var filePath = Path.Combine(Environment.CurrentDirectory, "Mystique_Plugins", moduleName, $"{moduleName}.dll");
                var referenceFolderPath = Path.GetDirectoryName(filePath);
                using (var fs = new FileStream(filePath, FileMode.Open))
                {
                    var assembly = context.LoadFromStream(fs);
                    _referenceLoader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                var controllerAssemblyPart = new MystiqueAssemblyPart(assembly);

                AdditionalReferencePathHolder.AdditionalReferencePaths.Add(filePath);
                _partManager.ApplicationParts.Add(controllerAssemblyPart);
                PluginsLoadContexts.AddPluginContext(moduleName, context);
            }
        }
        else
        {
            var context = PluginsLoadContexts.GetContext(moduleName);
            var controllerAssemblyPart = new MystiqueAssemblyPart(context.Assemblies.First());
            _partManager.ApplicationParts.Add(controllerAssemblyPart);
        }

        ResetControllActions();
    }

        public void DisableModule(string moduleName)
        {
            var last = _partManager.ApplicationParts.First(p => p.Name == moduleName);
            _partManager.ApplicationParts.Remove(last);

            ResetControllActions();
        }

        public void DeleteModule(string moduleName)
        {
            PluginsLoadContexts.RemovePluginContext(moduleName);

            var filePath = Path.Combine(Environment.CurrentDirectory, "Mystique_Plugins", moduleName, $"{moduleName}.dll");
            var referenceFolderPath = Path.GetDirectoryName(filePath);
            var directory = new DirectoryInfo(referenceFolderPath);
            directory.Delete(true);
        }

        private void ResetControllActions()
        {
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }
    }
}
