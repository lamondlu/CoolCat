using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
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
        private readonly IHttpContextAccessor _context;

        public MvcModuleSetup(ApplicationPartManager partManager, IReferenceLoader referenceLoader, IHttpContextAccessor httpContextAccessor)
        {
            _partManager = partManager;
            _referenceLoader = referenceLoader;
            _context = httpContextAccessor;
        }

        public void EnableModule(string moduleName)
        {
            if (!PluginsLoadContexts.Any(moduleName))
            {
                CollectibleAssemblyLoadContext context = new CollectibleAssemblyLoadContext(moduleName);

                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Modules", moduleName, $"{moduleName}.dll" );
                var viewFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName, $"{moduleName}.Views.dll");
                var referenceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Modules", moduleName);
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    var assembly = context.LoadFromStream(fs);
                    _referenceLoader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                    context.SetEntryPoint(assembly);

                    var controllerAssemblyPart = new MystiqueAssemblyPart(assembly);

                    AdditionalReferencePathHolder.AdditionalReferencePaths.Add(filePath);
                    _partManager.ApplicationParts.Add(controllerAssemblyPart);
                    PluginsLoadContexts.Add(moduleName, context);
                    context.Enable();
                }

                using (FileStream fsView = new FileStream(viewFilePath, FileMode.Open))
                {
                    var viewAssembly = context.LoadFromStream(fsView);
                    _referenceLoader.LoadStreamsIntoContext(context, referenceFolderPath, viewAssembly);

                    var moduleView = new MystiqueRazorAssemblyPart(viewAssembly, moduleName);
                    _partManager.ApplicationParts.Add(moduleView);
                }
            }
            else
            {
                var context = PluginsLoadContexts.Get(moduleName);
                var controllerAssemblyPart = new MystiqueAssemblyPart(context.Assemblies.First());
                _partManager.ApplicationParts.Add(controllerAssemblyPart);
                context.Enable();
            }

            ResetControllActions();
        }

        public void DisableModule(string moduleName)
        {
            var last = _partManager.ApplicationParts.First(p => p.Name == moduleName);
            _partManager.ApplicationParts.Remove(last);

            var context = PluginsLoadContexts.Get(moduleName);
            context.Disable();

            ResetControllActions();
        }

        public void DeleteModule(string moduleName)
        {
            PluginsLoadContexts.Remove(moduleName);

            var directory = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{moduleName}");
            directory.Delete(true);
        }

        private void ResetControllActions()
        {
            var provider = _context.HttpContext.RequestServices.GetService(typeof(IViewCompilerProvider)) as MystiqueViewCompilerProvider;
            provider.Refresh();
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }
    }
}
