using BookLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Mystique.Core.Attributes;
using Mystique.Core.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Controllers
{
    [Area(ModuleDefiniation.MODULE_NAME)]
    public class BookController : Controller
    {
        private readonly IDataStore _dataStore;

        public BookController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        [Page("Book Library")]
        public IActionResult AvailableBooks()
        {
            var books = JsonConvert.DeserializeObject<List<BookViewModel>>(_dataStore.Query("BookInventory", "Available_Books", string.Empty, source: ModuleDefiniation.MODULE_NAME));

            return View(books);
        }
    }
}
