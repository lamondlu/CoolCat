using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DynamicPlugins.Core.Contracts;
using DynamicPlugins.Core.DomainModel;
using DynamicPluginsDemoSite.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

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

        public IActionResult Enable()
        {
            var assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "Modules\\DemoPlugin1\\DemoPlugin1.dll");

            var controllerAssemblyPart = new AssemblyPart(assembly);
            _partManager.ApplicationParts.Add(controllerAssemblyPart);

            MyActionDescriptorChangeProvider.Instance.HasChanged = true;
            MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();

            return Content("Enabled");
        }

        public IActionResult Disable()
        {
            var last = _partManager.ApplicationParts.Last();
            _partManager.ApplicationParts.Remove(last);

            MyActionDescriptorChangeProvider.Instance.HasChanged = true;
            MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();

            return Content("Enabled");
        }
    }
}
