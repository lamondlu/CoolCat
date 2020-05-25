using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Mvc.Extensions;
using System;

namespace Mystique.Controllers
{
    public class PluginsController : Controller
    {
        private readonly IPluginManager _pluginManager = null;
        private readonly IReferenceContainer _referenceContainer = null;

        public PluginsController(IPluginManager pluginManager, IReferenceContainer referenceContainer)
        {
            _pluginManager = pluginManager;
            _referenceContainer = referenceContainer;
        }

        public IActionResult Assemblies()
        {
            System.Collections.Generic.List<CachedReferenceItemKey> items = _referenceContainer.GetAll();

            return View(items);
        }

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
            PluginPackage package = new PluginPackage(Request.GetPluginStream());
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
