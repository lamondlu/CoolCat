using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using CoolCat.Core.Contracts;
using CoolCat.Core.Mvc.Infrastructure;
using CoolCat.Mvc.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace CoolCat.Core.Mvc
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
            //ServiceProvider provider = CoolCatStartup.Services.BuildServiceProvider();
            //var contextProvider = new CollectibleAssemblyLoadContextProvider();

            //using (IServiceScope scope = provider.CreateScope())
            //{
            //    var dataStore = scope.ServiceProvider.GetService<IDataStore>();
            //    var documentation = scope.ServiceProvider.GetService<IQueryDocumentation>();

            //    var context = contextProvider.Get(moduleName, _partManager, scope, dataStore, documentation);
            //    PluginsLoadContexts.Add(moduleName, context);
            //    context.Enable();
            //}

            //ResetControllActions();

            if (!PluginsLoadContexts.Any(moduleName))
            {
                ServiceProvider provider = CoolCatStartup.Services.BuildServiceProvider();
                var contextProvider = new CollectibleAssemblyLoadContextProvider();

                using (IServiceScope scope = provider.CreateScope())
                {
                    var dataStore = scope.ServiceProvider.GetService<IDataStore>();
                    var documentation = scope.ServiceProvider.GetService<IQueryDocumentation>();

                    var context = contextProvider.Get(moduleName, _partManager, scope, dataStore, documentation);
                    PluginsLoadContexts.Add(moduleName, context);
                }

                ResetControllActions();
            }
        }

        public void DisableModule(string moduleName)
        {
            var controller = _partManager.ApplicationParts.First(p => p.Name == moduleName);
            _partManager.ApplicationParts.Remove(controller);

            var ui = _partManager.ApplicationParts.First(p => p.Name == $"{moduleName}.Views");
            _partManager.ApplicationParts.Remove(ui);

            var context = PluginsLoadContexts.Get(moduleName);
            context.Disable();

            //PluginsLoadContexts.Remove(moduleName);

            ResetControllActions();
        }

        public void DeleteModule(string moduleName)
        {
            PluginsLoadContexts.Remove(moduleName);

            var directory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName));
            directory.Delete(true);
        }

        private void ResetControllActions()
        {
            var provider = _context.HttpContext.RequestServices.GetService(typeof(IViewCompilerProvider)) as CoolCatViewCompilerProvider;
            provider.Refresh();
            CoolCatActionDescriptorChangeProvider.Instance.HasChanged = true;
            CoolCatActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }
    }
}
