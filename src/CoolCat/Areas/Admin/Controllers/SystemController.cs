﻿using Microsoft.AspNetCore.Mvc;
using CoolCat.Core;
using CoolCat.Core.Consts;
using CoolCat.Core.Contracts;
using CoolCat.Core.DTOs;
using CoolCat.Models;
using CoolCat.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace CoolCat.Controllers
{
    [Area("Admin")]
    public class SystemController : Controller
    {
        private readonly ISystemManager _systemManager;
        private readonly IPluginManager _pluginManager;
        private readonly IDbHelper _dbHelper;


        public SystemController(ISystemManager systemManager, IPluginManager pluginManager, IDbHelper dbHelper)
        {
            _systemManager = systemManager;
            _pluginManager = pluginManager;
            _dbHelper = dbHelper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDTO dto)
        {
            return View();
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
                        _pluginManager.AddPlugins(new Core.DomainModel.PluginPackage(fs, _dbHelper));
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