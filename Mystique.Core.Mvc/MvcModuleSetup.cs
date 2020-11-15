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

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Modules", moduleName, $"{moduleName}.dll" );
                string viewFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName, $"{moduleName}.Views.dll");
                string referenceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Modules", moduleName);
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    System.Reflection.Assembly assembly = context.LoadFromStream(fs);
                    _referenceLoader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                    context.SetEntryPoint(assembly);

                    MystiqueAssemblyPart controllerAssemblyPart = new MystiqueAssemblyPart(assembly);

                    AdditionalReferencePathHolder.AdditionalReferencePaths.Add(filePath);
                    _partManager.ApplicationParts.Add(controllerAssemblyPart);
                    PluginsLoadContexts.Add(moduleName, context);
                    context.Enable();
                }

                using (FileStream fs1 = new FileStream(viewFilePath, FileMode.Open))
                {
                    System.Reflection.Assembly assembly = context.LoadFromStream(fs1);

                    MystiqueRazorAssemblyPart controllerAssemblyPart = new MystiqueRazorAssemblyPart(assembly, moduleName);
                    _partManager.ApplicationParts.Add(controllerAssemblyPart);
                }
            }
            else
            {
                CollectibleAssemblyLoadContext context = PluginsLoadContexts.Get(moduleName);
                MystiqueAssemblyPart controllerAssemblyPart = new MystiqueAssemblyPart(context.Assemblies.First());
                _partManager.ApplicationParts.Add(controllerAssemblyPart);
                context.Enable();
            }

            ResetControllActions();
        }

        public void DisableModule(string moduleName)
        {
            ApplicationPart last = _partManager.ApplicationParts.First(p => p.Name == moduleName);
            _partManager.ApplicationParts.Remove(last);

            CollectibleAssemblyLoadContext context = PluginsLoadContexts.Get(moduleName);
            context.Disable();

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
            var a = _context.HttpContext.RequestServices.GetService(typeof(IViewCompilerProvider)) as MyViewCompilerProvider;
            a.Modify();
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }
    }
}
