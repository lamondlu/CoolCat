using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Mystique.Core;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Mvc.Extensions;
using Mystique.Mvc.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace Mystique.Controllers
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

        private void RefreshControllerAction()
        {
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
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
            var package = new PluginPackage(Request.GetPluginStream());
            _pluginManager.AddPlugins(package);
            return RedirectToAction("Index");
        }

        public IActionResult Enable(Guid id)
        {
            _pluginManager.EnablePlugin(id);
            return RedirectToAction("Index");
        }

        public IActionResult Disable(Guid id)
        {
            _pluginManager.DisablePlugin(id);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(Guid id)
        {
            _pluginManager.DeletePlugin(id);

            return RedirectToAction("Index");
        }
    }
}
