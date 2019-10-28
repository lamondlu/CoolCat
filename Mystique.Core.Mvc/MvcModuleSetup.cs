using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Mystique.Core.Contracts;
using Mystique.Core.Helpers;
using Mystique.Core.Mvc.Infrastructure;
using Mystique.Mvc.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mystique.Core.Mvc
{
    public class MvcModuleSetup : IMvcModuleSetup
    {
        private readonly ApplicationPartManager partManager;
        private readonly IReferenceLoader referenceLoader;

        public MvcModuleSetup(ApplicationPartManager partManager, IReferenceLoader referenceLoader)
        {
            this.partManager = partManager;
            this.referenceLoader = referenceLoader;
        }

        public async Task EnableModuleAsync(string moduleName)
        {
            if (!PluginsLoadContexts.Any(moduleName))
            {
                var context = new CollectibleAssemblyLoadContext();

                var filePath = Path.Combine(Environment.CurrentDirectory, "Mystique_plugins", moduleName, $"{moduleName}.dll");
                var referenceFolderPath = Path.GetDirectoryName(filePath);
                using (var fs = new FileStream(filePath, FileMode.Open))
                {
                    var assembly = context.LoadFromStream(fs);
                    referenceLoader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                    var controllerAssemblyPart = new MystiqueAssemblyPart(assembly);

                    AdditionalReferencePathHolder.AdditionalReferencePaths.Add(filePath);
                    partManager.ApplicationParts.Add(controllerAssemblyPart);
                    PluginsLoadContexts.AddPluginContext(moduleName, context);
                }
            }
            else
            {
                var context = PluginsLoadContexts.GetContext(moduleName);
                var controllerAssemblyPart = new MystiqueAssemblyPart(context.Assemblies.First());
                partManager.ApplicationParts.Add(controllerAssemblyPart);
            }

            await ResetControllerActionsAsync();
        }

        public async Task DisableModuleAsync(string moduleName)
        {
            var find = partManager.ApplicationParts.First(p => p.Name == moduleName);
            if (find != null)
            {
                partManager.ApplicationParts.Remove(find);
                await ResetControllerActionsAsync();
            }
        }

        public async Task DeleteModuleAsync(string moduleName)
        {
            PluginsLoadContexts.RemovePluginContext(moduleName);

            await Task.Run(() =>
            {
                var directory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Mystique_plugins", moduleName));
                if (directory.Exists)
                {
                    directory.Delete(true);
                }
            });
        }

        private Task ResetControllerActionsAsync()
        {
            return Task.Run(() =>
            {
                MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
                MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
            });
        }
    }
}
