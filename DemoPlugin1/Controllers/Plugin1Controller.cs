using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;
using Mystique.Core.Attributes;
using Mystique.Core.Contracts;
using Mystique.Core.Models;
using Newtonsoft.Json;

namespace DemoPlugin1.Controllers
{
    [Area("DemoPlugin1")]
    public class Plugin1Controller : Controller
    {
        private INotificationRegister _notificationRegister;

        public Plugin1Controller(INotificationRegister notificationRegister)
        {
            _notificationRegister = notificationRegister;
        }

        [Page("Plugin One")]
        [HttpGet]
        public IActionResult HelloWorld()
        {
            string content = new Demo().SayHello();
            ViewBag.Content = content + "; Plugin2 triggered";

            _notificationRegister.Publish("LoadHelloWorldEvent", JsonConvert.SerializeObject(new LoadHelloWorldEvent() { Str = "Hello World" }));

            return View();
        }
    }

    public class LoadHelloWorldEvent
    {
        public string Str { get; set; }
    }
}
