using CoolCat.Core.Attributes;
using CoolCat.Core.Contracts;
using CoolCat.Core.Mvc.Infrastructure;
using DemoPlugin1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DemoPlugin1.Controllers
{
    [Area(ModuleDefiniation.MODULE_NAME)]
    public class Plugin1Controller : CoolCatController
    {
        private readonly INotificationRegister _notificationRegister;
        private readonly IDataStore _dataStore;

        public Plugin1Controller(INotificationRegister notificationRegister, IDataStore dataStore) : base(ModuleDefiniation.MODULE_NAME, dataStore)
        {
            _notificationRegister = notificationRegister;
            _dataStore = dataStore;
        }

        [Page("Plugin One")]
        [HttpGet]
        public IActionResult HelloWorld()
        {
            TestClass testClass = new TestClass
            {
                Message = "Hello World"
            };

            _notificationRegister.Publish("LoadHelloWorldEvent", JsonConvert.SerializeObject(new LoadHelloWorldEvent() { Str = "Hello World" }));

            ViewBag.Books = JsonConvert.DeserializeObject<List<BookViewModel>>(_dataStore.Query("BookInventory", "Available_Books", string.Empty, source: ModuleDefiniation.MODULE_NAME));

            return View(testClass);
        }

        [HttpGet]
        public IActionResult Register()
        {
            CoolCatStartup.Services.AddScoped<IHandler, MyHandler>();
            return Content("OK");
        }

        [HttpGet]
        public IActionResult Show()
        {
            ServiceProvider provider = CoolCatStartup.Services.BuildServiceProvider();
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
