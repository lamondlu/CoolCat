using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;

namespace DemoPlugin2.Controllers
{
    [Area("DemoPlugin2")]
    public class Plugin1Controller : Controller
    {
        public IActionResult HelloWorld()
        {
            var content = new Demo().SayHello();
            ViewBag.Content = content;
            return View();
        }
    }
}
