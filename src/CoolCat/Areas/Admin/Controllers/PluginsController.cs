using Microsoft.AspNetCore.Mvc;
using CoolCat.Core.Contracts;
using CoolCat.Core.DomainModel;
using CoolCat.Core.Mvc.Extensions;
using System;
using Microsoft.AspNetCore.Authorization;

namespace CoolCat.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PluginsController : Controller
    {
        private readonly IPluginManager _pluginManager = null;
        private readonly IReferenceContainer _referenceContainer = null;
        private readonly IDbHelper _dbHelper = null;
        private readonly IQueryDocumentation _queryDocumentation = null;

        public PluginsController(IPluginManager pluginManager, IReferenceContainer referenceContainer, IDbHelper dbHelper, IQueryDocumentation queryDocumentation)
        {
            _pluginManager = pluginManager;
            _referenceContainer = referenceContainer;
            _dbHelper = dbHelper;
            _queryDocumentation = queryDocumentation;
        }


        public IActionResult Document()
        {
            var documents = _queryDocumentation.GetAllDocuments();
            return View(documents);
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
            PluginPackage package = new PluginPackage(Request.GetPluginStream(), _dbHelper);
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
