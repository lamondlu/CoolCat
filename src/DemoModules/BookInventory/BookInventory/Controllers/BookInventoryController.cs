using BookInventory.DAL;
using BookInventory.Dtos;
using BookInventory.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CoolCat.Core.Attributes;
using CoolCat.Core.Contracts;
using System;

namespace BookInventory.Controllers
{
    [Area(ModuleDefiniation.MODULE_NAME)]
    public class BookInventoryController : Controller
    {
        private BookDAL _bookDAL = null;
        private IDbHelper _dbHelper = null;

        public BookInventoryController(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
            _bookDAL = new BookDAL(_dbHelper);
        }

        [Page("Book Inventory")]
        [HttpGet]
        public IActionResult Books()
        {
            var result = _bookDAL.GetBooks();

            return View(result);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddBookDto dto)
        {
            if (dto == null || !ModelState.IsValid)
            {
                return View();
            }

            _bookDAL.AddBook(dto);

            return RedirectToAction("Books", "BookInventory", new { Area = ModuleDefiniation.MODULE_NAME });

        }

        [HttpDelete]
        public IActionResult Delete(Guid bookId)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(Guid bookId)
        {
            return View(new BookDetailViewModel());
        }

        [HttpPut]
        public IActionResult Update(Guid bookId, [FromForm] UpdateBookDto dto)
        {
            return View();
        }
    }
}
