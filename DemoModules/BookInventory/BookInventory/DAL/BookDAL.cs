using BookInventory.Dtos;
using BookInventory.ViewModels;
using Mystique.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BookInventory.DAL
{
    public class BookDAL
    {
        private DbHelper _dbHelper = null;

        public BookDAL(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<BookListViewModel> GetBooks()
        {
            var sql = "SELECT * FROM book";

            var dataTable = _dbHelper.ExecuteDataTable(sql);

            return dataTable.Rows.Cast<DataRow>().Select(p => new BookListViewModel
            {
                BookId = Guid.Parse(p["BookId"].ToString()),
                BookName = p["BookName"].ToString(),
                ISBN = p["ISBN"].ToString(),
                DateIssued = Convert.ToDateTime(p["DateIssued"])
            }).ToList();
        }

        public void AddBook(AddBookDto dto)
        {

        }

        public void UpdateBook(Guid bookId, UpdateBookDto dto)
        {

        }

        public void DeleteBook(Guid bookId)
        {

        }

        public void UpdateBookStatus(Guid bookId)
        {

        }
    }
}
