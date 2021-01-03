using BookInventory.Dtos;
using BookInventory.ViewModels;
using CoolCat.Core.Contracts;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace BookInventory.DAL
{
    public class BookDAL : IDisposable
    {
        private IDbConnection _dbConnection = null;

        public BookDAL(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnection = dbConnectionFactory.GetConnection();
        }

        public List<BookListViewModel> GetBooks()
        {
            var sql = "SELECT * FROM book";

            var books = _dbConnection.Query<BookListViewModel>(sql).ToList();
            return books;
        }

        public void AddBook(AddBookDto dto)
        {
            var sql = @"INSERT INTO Book(BookId, BookName, ISBN, DateIssued, Description)
                        VALUES(@id, @bookName, @isbn, @dateIssued, @description)";

            _dbConnection.Execute(sql, new
            {
                id = Guid.NewGuid(),
                bookName = dto.BookName,
                isbn = dto.ISBN,
                dateIssued = dto.DateIssued,
                description = dto.Description
            });
        }

        public BookDetailViewModel GetBook(Guid bookId)
        {
            var sql = "SELECT * FROM Book WHERE BookId=@id";
            var book = _dbConnection.QueryFirstOrDefault<BookDetailViewModel>(sql, new { id = bookId });
            return book;
        }

        public void UpdateBook(Guid bookId, UpdateBookDto dto)
        {
            var sql = "UPDATE Book SET BookName=@bookName, ISBN=@isbn, DateIssued=@dateIssued, Description=@description WHERE BookId=@id";

            _dbConnection.Execute(sql, new
            {
                id = bookId,
                bookName = dto.BookName,
                isbn = dto.ISBN,
                dateIssued = dto.DateIssued,
                description = dto.Description
            });
        }

        public void DeleteBook(Guid bookId)
        {
            var sql = "DELETE FROM Book WHERE BookId=@id";

            _dbConnection.Execute(sql, new
            {
                id = bookId
            });
        }

        public void UpdateBookStatus(Guid bookId, bool isOut)
        {
            var sql = "UPDATE Book SET Status=@status WHERE BookId=@id";

            _dbConnection.Execute(sql, new
            {
                id = bookId,
                status = isOut
            });
        }

        public void Dispose()
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
            _dbConnection = null;
        }
    }
}
