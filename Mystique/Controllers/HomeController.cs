using Microsoft.AspNetCore.Mvc;
using Mystique.Core.Contracts;
using Mystique.Models;
using System.Diagnostics;

namespace Mystique.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPluginManager _pluginManager = null;
        private readonly ISystemManager _systemManager = null;

        public HomeController(IPluginManager pluginManager, ISystemManager systemManager)
        {
            _pluginManager = pluginManager;
            _systemManager = systemManager;
        }

        public IActionResult Index()
        {
            if (_systemManager.CheckInstall())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Setup", "System");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
