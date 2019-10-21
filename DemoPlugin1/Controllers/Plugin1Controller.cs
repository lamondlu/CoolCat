using DemoReferenceLibrary;
using Microsoft.AspNetCore.Mvc;
using test1;

namespace DemoPlugin1.Controllers
{
    [Area("DemoPlugin1")]
    public class Plugin1Controller : Controller
    {
        public IActionResult HelloWorld()
        {
            Class1 c = new Class1();
            var content = new Demo().SayHello();
            ViewBag.Content = content;
            return View();
        }
    }
}
