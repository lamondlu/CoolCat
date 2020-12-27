using CoolCat.Core.Attributes;
using CoolCat.Core.Mvc.Infrastructure;
using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;

namespace DemoPlugin2.Controllers
{
    [Area("DemoPlugin2")]
    public class Plugin2Controller : CoolCatController
    {
        public Plugin2Controller() : base("DemoPlugin2", null)
        {

        }


        [Page("Plugin Two")]
        [HttpGet]
        public IActionResult HelloWorld()
        {
            string content = new Demo().SayHello() + "Version";
            ViewBag.Content = content;
            return View();
        }
    }
}
