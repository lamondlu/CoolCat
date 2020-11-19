using BookInventory.DAL;
using BookInventory.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mystique.Core.Helpers;
using Mystique.Core.Models;
using System;

namespace BookInventory.Controllers
{
    public class BookInventoryController : Controller
    {
        private BookDAL _bookDAL = null;
        private DbHelper _dbHelper = null;

        public BookInventoryController(IOptions<ConnectionStringSetting> connectionStringAccessor)
        {
            _dbHelper = new DbHelper(connectionStringAccessor.Value.ConnectionString);
            _bookDAL = new BookDAL(_dbHelper);
        }

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
