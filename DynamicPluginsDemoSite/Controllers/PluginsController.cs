using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicPlugins.Core.Contracts;
using DynamicPlugins.Core.DomainModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPluginsDemoSite.Controllers
{
    public class PluginsController : Controller
    {
        private IPluginManager _pluginManager = null;

        public PluginsController(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(_pluginManager.GetAllPlugins());
        }

        [HttpPost()]
        public IActionResult Add(IFormFileCollection files)
        {
            if (!files.Any())
            {
                ModelState.AddModelError("", "The plugin package file is missing.");
            }
            else
            {
                var package = new PluginPackage(files.First().OpenReadStream());
                _pluginManager.AddPlugins(package);
            }

            return View();
        }
    }
}
