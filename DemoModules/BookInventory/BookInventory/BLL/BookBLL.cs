using BookInventory.DAL;
using BookInventory.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookInventory.BLL
{
    public class BookBLL
    {
        private BookDAL bookDAL;

        public BookBLL()
        {
            bookDAL = new BookDAL();
        }

        public List<BookListViewModel> GetBooks()
        {
            return bookDAL.GetBooks();
        }
    }
}
