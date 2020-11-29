using Microsoft.AspNetCore.Mvc;
using Mystique.Core;
using Mystique.Core.Consts;
using Mystique.Core.Contracts;
using Mystique.Core.DTOs;
using Mystique.Models;
using Mystique.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mystique.Controllers
{
    public class SystemController : Controller
    {
        private readonly ISystemManager _systemManager;
        private readonly IPluginManager _pluginManager;


        public SystemController(ISystemManager systemManager, IPluginManager pluginManager)
        {
            _systemManager = systemManager;
            _pluginManager = pluginManager;
        }

        public IActionResult GetSiteCSS()
        {
            var settings = _systemManager.GetSiteSettings();

            if (!string.IsNullOrEmpty(settings.SiteCSS))
            {
                return Content(settings.SiteCSS, "text/css");
            }

            return null;
        }

        [HttpGet("~/System/GetModuleCSS")]
        public IActionResult GetModuleCSS(string moduleName, string fileName)
        {
            var stream = PluginsLoadContexts.Get(moduleName).LoadResource(fileName);

            return new FileStreamResult(stream, "text/css");
        }

        [HttpGet("~/System/GetModuleScript")]
        public IActionResult GetModuleScript(string moduleName, string fileName)
        {
            var stream = PluginsLoadContexts.Get(moduleName).LoadResource(fileName);
            return new FileStreamResult(stream, "text/javascript");
        }

        public IActionResult Setup()
        {
            PresetPluginLoader presetPluginLoader = new PresetPluginLoader();
            List<string> plugins = presetPluginLoader.LoadPlugins();

            if (plugins.Count == 0)
            {
                _systemManager.MarkAsInstalled();
                return RedirectToAction("Index", "Home");
            }

            return View(plugins);
        }

        [HttpPost]
        public IActionResult Install([FromBody] SetupModulesModel model)
        {
            if (model != null && model.Modules != null)
            {
                foreach (string module in model.Modules)
                {
                    using (FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GlobalConst.PresetFolder, module), FileMode.Open))
                    {
                        _pluginManager.AddPlugins(new Core.DomainModel.PluginPackage(fs));
                    }
                }
            }

            _systemManager.MarkAsInstalled();

            return Ok();
        }

        [HttpGet("SiteSettings")]
        public IActionResult SiteSettings()
        {
            var settings = _systemManager.GetSiteSettings();
            return View(settings);
        }

        [HttpPost("SiteSettings")]
        public IActionResult SiteSettings(SiteSettingsDTO dto)
        {
            _systemManager.SaveSiteSettings(dto);

            var settings = _systemManager.GetSiteSettings();
            return View(settings);
        }


    }
}
