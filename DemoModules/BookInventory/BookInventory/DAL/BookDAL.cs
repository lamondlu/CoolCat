using BookInventory.Dtos;
using BookInventory.ViewModels;
using MySql.Data.MySqlClient;
using Mystique.Core.Contracts;
using Mystique.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BookInventory.DAL
{
    public class BookDAL
    {
        private IDbHelper _dbHelper = null;

        public BookDAL(IDbHelper dbHelper)
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
            var sql = "INSERT INTO Book(BookId, BookName, ISBN, DateIssued, Description) VALUES(@id, @bookName, @isbn, @dateIssued, @description)";

            _dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter>
            {
                new MySqlParameter { ParameterName = "@id", MySqlDbType = MySqlDbType.Guid, Value = Guid.NewGuid() },
                new MySqlParameter { ParameterName = "@bookName", MySqlDbType = MySqlDbType.VarChar, Value = dto.BookName },
                new MySqlParameter { ParameterName = "@isbn", MySqlDbType = MySqlDbType.VarChar, Value = dto.ISBN },
                new MySqlParameter { ParameterName = "@dateIssued", MySqlDbType = MySqlDbType.Date, Value = dto.DateIssued },
                new MySqlParameter { ParameterName = "@description", MySqlDbType = MySqlDbType.Text, Value = dto.Description }
            }.ToArray());
        }

        public BookDetailViewModel GetBook(Guid bookId)
        {
            var sql = "SELECT * FROM Book WHERE BookId=@id";

            var dataTables = _dbHelper.ExecuteDataTable(sql, new MySqlParameter { ParameterName = "@id", MySqlDbType = MySqlDbType.Guid, Value = bookId });

            if (dataTables.Rows.Count == 0)
            {
                return dataTables.Rows.Cast<DataRow>().Select(p => new BookDetailViewModel
                {
                    BookId = Guid.Parse(p["BookId"].ToString()),
                    BookName = p["BookName"].ToString(),
                    DateIssued = Convert.ToDateTime(p["DateIssued"]),
                    Description = p["Description"].ToString(),
                    ISBN = p["ISBN"].ToString()
                }).First();
            }
            else
            {
                return null;
            }
        }

        public void UpdateBook(Guid bookId, UpdateBookDto dto)
        {
            var sql = "UPDATE SET Book BookName=@bookName, ISBN=@isbn, DateIssued=@dateIssued, Description=@description WHERE BookId=@id";

            _dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter>
            {
                new MySqlParameter { ParameterName = "@id", MySqlDbType = MySqlDbType.Guid, Value = bookId },
                new MySqlParameter { ParameterName = "@bookName", MySqlDbType = MySqlDbType.VarChar, Value = dto.BookName },
                new MySqlParameter { ParameterName = "@isbn", MySqlDbType = MySqlDbType.VarChar, Value = dto.ISBN },
                new MySqlParameter { ParameterName = "@dateIssued", MySqlDbType = MySqlDbType.Date, Value = dto.DateIssued },
                new MySqlParameter { ParameterName = "@description", MySqlDbType = MySqlDbType.Text, Value = dto.Description }
            }.ToArray());
        }

        public void DeleteBook(Guid bookId)
        {
            var sql = "DELETE FROM Book WHERE BookId=@id";

            _dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter>
            {
                new MySqlParameter { ParameterName = "@id", MySqlDbType = MySqlDbType.Guid, Value = bookId },
            }.ToArray());
        }

        public void UpdateBookStatus(Guid bookId, bool isOut)
        {
            var sql = "UPDATE SET Book SET Status=@status WHERE BookId=@id";

            _dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter>
            {
                new MySqlParameter { ParameterName = "@id", MySqlDbType = MySqlDbType.Guid, Value = bookId },
                new MySqlParameter { ParameterName = "@status", MySqlDbType = MySqlDbType.Bit, Value = isOut },
            }.ToArray());
        }
    }
}
