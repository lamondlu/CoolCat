using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;
using CoolCat.Core.Attributes;

namespace DemoPlugin2.Controllers
{
    [Area("DemoPlugin2")]
    public class Plugin2Controller : Controller
    {

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
