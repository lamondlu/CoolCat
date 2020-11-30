using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
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
                ServiceProvider provider = MystiqueStartup.Services.BuildServiceProvider();
                var contextProvider = new CollectibleAssemblyLoadContextProvider();

                using (IServiceScope scope = provider.CreateScope())
                {
                    var dataStore = scope.ServiceProvider.GetService<IDataStore>();

                    var context = contextProvider.Get(moduleName, _partManager, scope, dataStore);
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
            var provider = _context.HttpContext.RequestServices.GetService(typeof(IViewCompilerProvider)) as MystiqueViewCompilerProvider;
            provider.Refresh();
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }
    }
}
