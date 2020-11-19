using BookInventory.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookInventory.Controllers
{
    public class BookInventoryController : Controller
    {
        public BookInventoryController()
        {

        }

        [HttpGet]
        public IActionResult Books()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddBookDto dto)
        {
            return View();
        }

        [HttpDelete]
        public IActionResult Delete(Guid bookId)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(Guid bookId)
        {
            return View();
        }

        [HttpPut]
        public IActionResult Update(Guid bookId, [FromForm] UpdateBookDto dto)
        {
            return View();
        }
    }
}
