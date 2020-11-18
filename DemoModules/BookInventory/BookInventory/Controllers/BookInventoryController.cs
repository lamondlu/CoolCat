using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookInventory.Controllers
{
    public class BookInventoryController : Controller
    {
        [HttpGet]
        public IActionResult Books()
        {
            return View();
        }
    }
}
