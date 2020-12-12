using BookLibrary.DAL;
using BookLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CoolCat.Core.Attributes;
using CoolCat.Core.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BookLibrary.Controllers
{
    [Area(ModuleDefiniation.MODULE_NAME)]
    public class BookController : Controller
    {
        private readonly IDataStore _dataStore;
        private readonly IDbHelper _dbHelper;
        private readonly BookDAL _bookDAL;
        private readonly INotificationRegister _notificationRegister;

        public BookController(IDataStore dataStore, IDbHelper dbHelper, INotificationRegister notificationRegister)
        {
            _dataStore = dataStore;
            _dbHelper = dbHelper;
            _bookDAL = new BookDAL(_dbHelper);
            _notificationRegister = notificationRegister;
        }

        [HttpGet]
        [Page("Book Library")]
        public IActionResult AvailableBooks()
        {
            var books = JsonConvert.DeserializeObject<List<BookListViewModel>>(_dataStore.Query("BookInventory", "Available_Books", string.Empty, source: ModuleDefiniation.MODULE_NAME));

            return View(books);
        }

        [HttpPut]
        public IActionResult RentBook(Guid bookId)
        {
            var book = JsonConvert.DeserializeObject<BookDetailsViewModel>(_dataStore.Query("BookInventory", "Book_Details", JsonConvert.SerializeObject(new { bookId }), source: ModuleDefiniation.MODULE_NAME));

            var date = DateTime.Today;

            _bookDAL.RentBook(new Dtos.RentBookDTO
            {
                BookId = bookId,
                BookName = book.BookName,
                DateIssued = book.DateIssued,
                ISBN = book.ISBN,
                RentDate = date
            });

            _notificationRegister.Publish("BookOutEvent", JsonConvert.SerializeObject(new
            {
                BookId = bookId,
                OutDate = date
            }));

            return Json(new
            {
                result = true
            });
        }

        [HttpPut]
        public IActionResult ReturnBook(Guid rentId)
        {
            _bookDAL.ReturnBook(new Dtos.ReturnBookDTO
            {
                RentId = rentId,
                ReturnDate = DateTime.Now
            });

            return Json(new
            {
                result = true
            });
        }
    }
}
