using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicPlugins.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
    }
}
