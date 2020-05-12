using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;
using Mystique.Core.Contracts;
using Mystique.Core.Models;

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

            _notificationRegister.Publish(new LoadHelloWorldEvent());

            return View();
        }
    }

    public class LoadHelloWorldEvent : EventBase
    {
        public LoadHelloWorldEvent() : base("LoadHelloWorldEvent")
        {

        }
    }
}
