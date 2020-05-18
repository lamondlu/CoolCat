using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult HelloWorld()
        {
            string content = new Demo().SayHello();
            ViewBag.Content = content;

            _notificationRegister.Publish("LoadHelloWorldEvent", JsonConvert.SerializeObject(new LoadHelloWorldEvent() { Str = "Hello World" }));

            return View();
        }
    }

    public class LoadHelloWorldEvent : EventBase
    {
        public string Str { get; set; }

        public LoadHelloWorldEvent() : base("LoadHelloWorldEvent")
        {

        }
    }
}
