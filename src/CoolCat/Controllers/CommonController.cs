using CoolCat.Core;
using CoolCat.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CoolCat.Controllers
{
    public class CommonController : Controller
    {
        private readonly ISystemManager _systemManager;
        private readonly IPluginManager _pluginManager;


        public CommonController(ISystemManager systemManager, IPluginManager pluginManager)
        {
            _systemManager = systemManager;
            _pluginManager = pluginManager;
        }

        [HttpGet("~/Common/GetSiteCSS")]
        public IActionResult GetSiteCSS()
        {
            var settings = _systemManager.GetSiteSettings();

            if (!string.IsNullOrEmpty(settings.SiteCSS))
            {
                return Content(settings.SiteCSS, "text/css");
            }

            return null;
        }


        [HttpGet("~/Common/GetModuleCSS")]
        public IActionResult GetModuleCSS(string moduleName, string fileName)
        {
            var fileContent = PluginsLoadContexts.Get(moduleName).LoadResource(fileName);

            return new FileContentResult(fileContent, "text/css");
        }

        [HttpGet("~/CommonS/GetModuleScript")]
        public IActionResult GetModuleScript(string moduleName, string fileName)
        {
            var fileContent = PluginsLoadContexts.Get(moduleName).LoadResource(fileName);
            return new FileContentResult(fileContent, "text/javascript");
        }
    }
}
