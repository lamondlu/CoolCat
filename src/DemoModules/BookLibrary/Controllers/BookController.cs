using BookLibrary.DAL;
using BookLibrary.ViewModels;
using CoolCat.Core.Attributes;
using CoolCat.Core.Contracts;
using CoolCat.Core.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BookLibrary.Controllers
{
    [Area(ModuleDefiniation.MODULE_NAME)]
    public class BookController : CoolCatController
    {
        private readonly BookDAL _bookDAL;
        private readonly INotificationRegister _notificationRegister;

        public BookController(IDataStore dataStore, IDbConnectionFactory dbConnectionFactory, INotificationRegister notificationRegister) : base(ModuleDefiniation.MODULE_NAME, dataStore)
        {
            _bookDAL = new BookDAL(dbConnectionFactory);
            _notificationRegister = notificationRegister;
        }

        [HttpGet]
        [Page("Book Library")]
        public IActionResult AvailableBooks()
        {
            var books = JsonConvert.DeserializeObject<List<BookListViewModel>>(Query("BookInventory", "Available_Books", string.Empty));

            return View(books);
        }

        [HttpPut]
        public IActionResult RentBook(Guid bookId)
        {
            var book = JsonConvert.DeserializeObject<BookDetailsViewModel>(Query("BookInventory", "Book_Details", JsonConvert.SerializeObject(new { bookId })));

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
            var bookId = _bookDAL.GetBookId(rentId);
            var date = DateTime.Today;

            _bookDAL.ReturnBook(new Dtos.ReturnBookDTO
            {
                RentId = rentId,
                ReturnDate = DateTime.Now
            });

            _notificationRegister.Publish("BookInEvent", JsonConvert.SerializeObject(new
            {
                BookId = bookId,
                OutDate = date
            }));

            return Json(new
            {
                result = true
            });
        }
    }
}
