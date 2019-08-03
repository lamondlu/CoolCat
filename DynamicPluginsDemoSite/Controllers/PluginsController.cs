using DynamicPlugins.Core.Contracts;
using DynamicPlugins.Core.DomainModel;
using DynamicPluginsDemoSite.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Linq;
using System.Reflection;

namespace DynamicPluginsDemoSite.Controllers
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
            var package = new PluginPackage(Request.Form.Files.First().OpenReadStream());
            _pluginManager.AddPlugins(package);

            return View("Add");
        }

        public IActionResult Enable(Guid id)
        {
            var module = _pluginManager.GetPlugin(id);
            _pluginManager.EnablePlugin(id);
            var moduleName = module.Name;

            var assembly = Assembly.LoadFile($"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{moduleName}\\{moduleName}.dll");

            var controllerAssemblyPart = new AssemblyPart(assembly);
            _partManager.ApplicationParts.Add(controllerAssemblyPart);

            MyActionDescriptorChangeProvider.Instance.HasChanged = true;
            MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();

            return RedirectToAction("Index");
        }

        public IActionResult Disable(Guid id)
        {
            var module = _pluginManager.GetPlugin(id);
            _pluginManager.DisablePlugin(id);
            var moduleName = module.Name;
            var moduleDLL = $"{ moduleName }.dll";

            var last = _partManager.ApplicationParts.First(p => p.Name == moduleDLL);
            _partManager.ApplicationParts.Remove(last);

            MyActionDescriptorChangeProvider.Instance.HasChanged = true;
            MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();

            return RedirectToAction("Index");
        }
    }
}
