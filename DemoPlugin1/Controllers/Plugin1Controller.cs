using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoPlugin1.Controllers
{
    public class Plugin1Controller : Controller
    {
        public IActionResult HelloWorld()
        {
            return View();
        }
    }
}
