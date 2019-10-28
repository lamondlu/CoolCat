using Microsoft.AspNetCore.Mvc;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Mvc.Extensions;
using Mystique.Mvc.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Mystique.Controllers
{
    public class PluginsController : Controller
    {
        private readonly IPluginManager pluginManager;
        private readonly IReferenceContainer referenceContainer;
        private readonly PluginPackage pluginPackage;

        public PluginsController(IPluginManager pluginManager, IReferenceContainer referenceContainer, PluginPackage pluginPackage)
        {
            this.pluginManager = pluginManager;
            this.referenceContainer = referenceContainer;
            this.pluginPackage = pluginPackage;
        }

        [HttpGet]
        public IActionResult RefreshControllerAction()
        {
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
            return Ok();
        }

        [HttpGet]
        public IActionResult Assemblies()
        {
            var items = referenceContainer.GetAll();
            return View(items);
        }

        [HttpGet("Index")]
        public async Task<IActionResult> IndexAsync()
        {
            var plugins = await pluginManager.GetAllPluginsAsync();
            return View(plugins);
        }

        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadAsync()
        {
            using (var stream = Request.GetPluginStream())
            {
                await pluginPackage.InitializeAsync(stream);
                await pluginManager.AddPluginsAsync(pluginPackage);
            }
            return RedirectToAction("Index");
        }

        [HttpGet("Enable")]
        public async Task<IActionResult> EnableAsync(Guid id)
        {
            await pluginManager.EnablePluginAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet("Disable")]
        public async Task<IActionResult> DisableAsync(Guid id)
        {
            await pluginManager.DisablePluginAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await pluginManager.DeletePluginAsync(id);
            return RedirectToAction("Index");
        }
    }
}
