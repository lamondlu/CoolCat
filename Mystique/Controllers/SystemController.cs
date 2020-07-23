using Microsoft.AspNetCore.Mvc;
using Mystique.Core.Contracts;
using Mystique.Models;
using Mystique.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mystique.Controllers
{
    public class SystemController : Controller
    {
        private ISystemManager _systemManager;
        private IPluginManager _pluginManager;


        public SystemController(ISystemManager systemManager, IPluginManager pluginManager)
        {
            _systemManager = systemManager;
            _pluginManager = pluginManager;
        }

        public IActionResult Setup()
        {
            var presetPluginLoader = new PresetPluginLoader();
            var plugins = presetPluginLoader.LoadPlugins();

            if (plugins.Count == 0)
            {
                _systemManager.MarkAsInstalled();
                return RedirectToAction("Index", "Home");
            }

            return View(plugins);
        }

        [HttpPost]
        public IActionResult Install([FromBody]SetupModulesModel model)
        {
            if (model != null && model.Modules != null)
            {
                foreach (var module in model.Modules)
                {
                    using (var fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PresetModules", module), FileMode.Open))
                    {
                        _pluginManager.AddPlugins(new Core.DomainModel.PluginPackage(fs));
                    }
                }
            }

            _systemManager.MarkAsInstalled();

            return Ok();
        }
    }
}
