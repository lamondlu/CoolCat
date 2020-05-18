using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;
using Mystique.Core.Attributes;

namespace DemoPlugin2.Controllers
{
    [Area("DemoPlugin2")]
    public class Plugin2Controller : Controller
    {

        [Page("Plugin Two")]
        [Route("HelloWorld")]
        public IActionResult HelloWorld()
        {
            string content = new Demo().SayHello() + "Version";
            ViewBag.Content = content;
            return View();
        }
    }
}
