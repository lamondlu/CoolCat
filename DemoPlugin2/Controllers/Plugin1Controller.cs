using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;

namespace DemoPlugin2.Controllers
{
    [Area("DemoPlugin2")]
    public class Plugin2Controller : Controller
    {
        public IActionResult HelloWorld()
        {
            string content = new Demo().SayHello() + "Version";
            ViewBag.Content = content;
            return View();
        }
    }
}
