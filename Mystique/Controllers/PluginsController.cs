using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Mvc.Extensions;
using Mystique.Mvc.Infrastructure;
using System;

namespace Mystique.Controllers
{
    public class PluginsController : Controller
    {
        private readonly IPluginManager pluginManager;
        private readonly PluginPackage pluginPackage;
        private readonly IReferenceContainer referenceContainer;

        public PluginsController(IPluginManager pluginManager, PluginPackage pluginPackage, IReferenceContainer referenceContainer)
        {
            this.pluginManager = pluginManager;
            this.pluginPackage = pluginPackage;
            this.referenceContainer = referenceContainer;
        }

        private void RefreshControllerAction()
        {
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }

        public IActionResult Assemblies()
        {
            var items = referenceContainer.GetAll();
            return View(items);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(pluginManager.GetAllPlugins());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload()
        {
            using (var stream = Request.GetPluginStream())
            {
                var package = this.pluginPackage;
                package.Initialize(stream);
                pluginManager.AddPlugins(package);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Enable(Guid id)
        {
            pluginManager.EnablePlugin(id);
            return RedirectToAction("Index");
        }

        public IActionResult Disable(Guid id)
        {
            pluginManager.DisablePlugin(id);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(Guid id)
        {
            pluginManager.DeletePlugin(id);
            return RedirectToAction("Index");
        }
    }
}
