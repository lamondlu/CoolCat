using BookLibrary.Dtos;
using CoolCat.Core.Contracts;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Dapper;

namespace BookLibrary.DAL
{
    public class BookDAL
    {
        private IDbConnectionFactory _dbConnectionFactory = null;

        public BookDAL(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public void RentBook(RentBookDTO dto)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var sql = @"INSERT INTO rent_history(RentId, BookId, BookName, ISBN, RentDate, DateIssued) 
                            VALUES(@rentId, @bookId, @bookName, @isbn, @rentDate, @dateIssued)";

                connection.Execute(sql, new
                {
                    rentId = Guid.NewGuid(),
                    bookId = dto.BookId,
                    bookName = dto.BookName,
                    isbn = dto.ISBN,
                    rentDate = dto.RentDate,
                    dateIssued = dto.DateIssued
                });
            }
        }

        public void ReturnBook(ReturnBookDTO dto)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var sql = "UPDATE rent_history SET ReturnDate=@returnDate WHERE RentId=@rentId";

                connection.Execute(sql, new { rentId = dto.RentId, returnDate = dto.ReturnDate });
            }
        }

        public Guid? GetBookId(Guid rentId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var sql = "SELECT BookId FROM rent_history WHERE RentId=@rentId";

                var result = connection.QueryFirstOrDefault<Guid>(sql, new { rentId });

                return result;
            }
        }
    }
}
