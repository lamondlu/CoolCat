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
        private readonly PluginPackage pluginPackage;

        public PluginsController(IPluginManager pluginManager, PluginPackage pluginPackage)
        {
            this.pluginManager = pluginManager;
            this.pluginPackage = pluginPackage;
        }

        [HttpGet]
        public IActionResult RefreshControllerAction()
        {
            MystiqueActionDescriptorChangeProvider.Instance.HasChanged = true;
            MystiqueActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
            return Ok();
        }

        [HttpGet("Index")]
        public async Task<IActionResult> IndexAsync()
        {
            var model = await pluginManager.GetAllPluginsAsync();
            return View(model);
        }

        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadAsync()
        {
            pluginPackage.Initialize(Request.GetPluginStream());
            await pluginManager.AddPluginsAsync(pluginPackage);
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
