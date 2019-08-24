using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Mystique.Core.Contracts;
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

        public MvcModuleSetup(ApplicationPartManager partManager)
        {
            _partManager = partManager;
        }

        public void EnableModule(string moduleName)
        {
            if (!PluginsLoadContexts.Any(moduleName))
            {
                var context = new CollectibleAssemblyLoadContext();

                var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{moduleName}\\{moduleName}.dll";
                using (var fs = new FileStream(filePath, FileMode.Open))
                {
                    var assembly = context.LoadFromStream(fs);

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

        private void ResetControllActions()
        {
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }
    }
}
