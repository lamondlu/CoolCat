using Microsoft.AspNetCore.Mvc;
using Mystique.Core.BusinessLogic;
using Mystique.Core.Contracts;
using Mystique.Core.Mvc.Extensions;
using Mystique.Models;
using System.Diagnostics;
using System.Linq;

namespace Mystique.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPluginManager _pluginManager = null;

        public HomeController(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public IActionResult Index()
        {
            var types = _pluginManager.GetAllContexts().SelectMany(p => p.GetPages()).ToList();

            return View(types);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
