using BookInventory.DAL;
using BookInventory.Dtos;
using BookInventory.ViewModels;
using CoolCat.Core.Attributes;
using CoolCat.Core.Contracts;
using CoolCat.Core.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BookInventory.Controllers
{
    [Area(ModuleDefiniation.MODULE_NAME)]
    public class BookInventoryController : CoolCatController
    {
        private BookDAL _bookDAL = null;

        public BookInventoryController(IDbConnectionFactory dbConnectionFactory, IDataStore dataStore) : base(ModuleDefiniation.MODULE_NAME, dataStore)
        {
            _bookDAL = new BookDAL(dbConnectionFactory);
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

            return RedirectToAction("Books", "BookInventory");
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
