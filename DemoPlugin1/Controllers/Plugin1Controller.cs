using DemoPlugin1.Models;
using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Mystique.Core.Attributes;
using Mystique.Core.Contracts;
using Mystique.Core.Mvc.Infrastructure;
using Newtonsoft.Json;

namespace DemoPlugin1.Controllers
{
    [Area("DemoPlugin1")]
    public class Plugin1Controller : Controller
    {
        private readonly INotificationRegister _notificationRegister;

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

            TestClass testClass = new TestClass
            {
                Message = "Hello World"
            };

            _notificationRegister.Publish("LoadHelloWorldEvent", JsonConvert.SerializeObject(new LoadHelloWorldEvent() { Str = "Hello World" }));

            return View(testClass);
        }

        [HttpGet]
        public IActionResult Register()
        {
            MystiqueStartup.Services.AddScoped<IHandler, MyHandler>();
            return Content("OK");
        }

        [HttpGet]
        public IActionResult Show()
        {
            ServiceProvider provider = MystiqueStartup.Services.BuildServiceProvider();
            using (IServiceScope scope = provider.CreateScope())
            {
                IHandler handler = scope.ServiceProvider.GetService<IHandler>();
                return Content(handler.Work());
            }

        }
    }

    public interface IHandler
    {
        string Work();
    }

    public class MyHandler : IHandler
    {
        public string Work()
        {
            return "My Handler Work";
        }
    }

    public class LoadHelloWorldEvent
    {
        public string Str { get; set; }
    }
}
