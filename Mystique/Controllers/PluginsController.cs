using Mystique.Core;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.IO;
using System.Linq;
using Mystique.Mvc.Infrastructure;
using Mystique.Core.Mvc.Infrastructure;
using Mystique.Core.Mvc.Extensions;

namespace Mystique.Controllers
{
    public class PluginsController : Controller
    {
        private IPluginManager _pluginManager = null;
        private ApplicationPartManager _partManager = null;

        public PluginsController(IPluginManager pluginManager, ApplicationPartManager partManager)
        {
            _pluginManager = pluginManager;
            _partManager = partManager;
        }

        private void RefreshControllerAction()
        {
            MyActionDescriptorChangeProvider.Instance.HasChanged = true;
            MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(_pluginManager.GetAllPlugins());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload()
        {
            var package = new PluginPackage(Request.GetPluginStream());
            _pluginManager.AddPlugins(package);

            return RedirectToAction("Index");
        }

        public IActionResult Enable(Guid id)
        {
            var module = _pluginManager.GetPlugin(id);
            if (!PluginsLoadContexts.Any(module.Name))
            {
                var context = new CollectibleAssemblyLoadContext();

                _pluginManager.EnablePlugin(id);
                var moduleName = module.Name;

                var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{moduleName}\\{moduleName}.dll";
                using (var fs = new FileStream(filePath, FileMode.Open))
                {
                    var assembly = context.LoadFromStream(fs);

                    var controllerAssemblyPart = new MystiqueAssemblyPart(assembly);

                    AdditionalReferencePathHolder.AdditionalReferencePaths.Add(filePath);
                    _partManager.ApplicationParts.Add(controllerAssemblyPart);
                }

                RefreshControllerAction();

                PluginsLoadContexts.AddPluginContext(module.Name, context);
            }
            else
            {
                var context = PluginsLoadContexts.GetContext(module.Name);
                var controllerAssemblyPart = new MystiqueAssemblyPart(context.Assemblies.First());
                _partManager.ApplicationParts.Add(controllerAssemblyPart);
                _pluginManager.EnablePlugin(id);

                RefreshControllerAction();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Disable(Guid id)
        {
            var module = _pluginManager.GetPlugin(id);
            _pluginManager.DisablePlugin(id);
            var moduleName = module.Name;

            var last = _partManager.ApplicationParts.First(p => p.Name == moduleName);
            _partManager.ApplicationParts.Remove(last);

            RefreshControllerAction();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(Guid id)
        {
            var module = _pluginManager.GetPlugin(id);
            _pluginManager.DisablePlugin(id);
            _pluginManager.DeletePlugin(id);
            var moduleName = module.Name;

            var matchedItem = _partManager.ApplicationParts.FirstOrDefault(p => p.Name == moduleName);

            if (matchedItem != null)
            {
                _partManager.ApplicationParts.Remove(matchedItem);
                matchedItem = null;
            }

            RefreshControllerAction();

            PluginsLoadContexts.RemovePluginContext(module.Name);

            var directory = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}Modules/{module.Name}");
            directory.Delete(true);

            return RedirectToAction("Index");
        }
    }
}
