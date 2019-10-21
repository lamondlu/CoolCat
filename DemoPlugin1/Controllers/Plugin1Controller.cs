using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;

namespace DemoPlugin1.Controllers
{
    [Area("DemoPlugin1")]
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
