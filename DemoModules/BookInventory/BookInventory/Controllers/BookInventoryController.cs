using BookInventory.DAL;
using BookInventory.Dtos;
using BookInventory.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mystique.Core.Attributes;
using Mystique.Core.Helpers;
using Mystique.Core.Models;
using System;

namespace BookInventory.Controllers
{
    [Area(ModuleDefiniation.MODULE_NAME)]
    public class BookInventoryController : Controller
    {
        private BookDAL _bookDAL = null;
        private DbHelper _dbHelper = null;

        public BookInventoryController(IOptions<ConnectionStringSetting> connectionStringAccessor)
        {
            _dbHelper = new DbHelper(connectionStringAccessor.Value.ConnectionString);
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

            return RedirectToAction("Books");
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
