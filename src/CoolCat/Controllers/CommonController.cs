using CoolCat.Core;
using CoolCat.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolCat.Controllers
{
    public class CommonController : Controller
    {
        private readonly ISystemManager _systemManager;
        private readonly IPluginManager _pluginManager;
        private readonly IDbHelper _dbHelper;


        public CommonController(ISystemManager systemManager, IPluginManager pluginManager, IDbHelper dbHelper)
        {
            _systemManager = systemManager;
            _pluginManager = pluginManager;
            _dbHelper = dbHelper;
        }


        [HttpGet("~/System/GetModuleCSS")]
        public IActionResult GetModuleCSS(string moduleName, string fileName)
        {
            var fileContent = PluginsLoadContexts.Get(moduleName).LoadResource(fileName);

            return new FileContentResult(fileContent, "text/css");
        }

        [HttpGet("~/System/GetModuleScript")]
        public IActionResult GetModuleScript(string moduleName, string fileName)
        {
            var fileContent = PluginsLoadContexts.Get(moduleName).LoadResource(fileName);
            return new FileContentResult(fileContent, "text/javascript");
        }
    }
}
