using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mystique.Controllers
{
    public class SystemController : Controller
    {
        public IActionResult Setup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Install()
        {
            return Ok();
        }
    }
}
