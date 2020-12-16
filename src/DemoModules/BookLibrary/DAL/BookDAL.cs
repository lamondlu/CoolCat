using BookLibrary.Dtos;
using MySql.Data.MySqlClient;
using CoolCat.Core.Contracts;
using System;
using System.Collections.Generic;

namespace BookLibrary.DAL
{
    public class BookDAL
    {
        private IDbHelper _dbHelper = null;

        public BookDAL(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public void RentBook(RentBookDTO dto)
        {
            var sql = "INSERT INTO rent_history(RentId, BookId, BookName, ISBN, RentDate, DateIssued) VALUES(@rentId, @bookId, @bookName, @isbn, @rentDate, @dateIssued)";

            _dbHelper.ExecuteNonQuery(sql,
                new List<MySqlParameter> {
                   new MySqlParameter { ParameterName = "@rentId", MySqlDbType = MySqlDbType.Guid, Value = Guid.NewGuid() },
                   new MySqlParameter { ParameterName = "@bookId", MySqlDbType = MySqlDbType.Guid, Value = dto.BookId },
                   new MySqlParameter { ParameterName = "@bookName", MySqlDbType = MySqlDbType.VarChar, Value = dto.BookName },
                   new MySqlParameter { ParameterName = "@isbn", MySqlDbType = MySqlDbType.VarChar, Value = dto.ISBN },
                   new MySqlParameter { ParameterName = "@rentDate", MySqlDbType = MySqlDbType.Date, Value = dto.RentDate },
                   new MySqlParameter { ParameterName = "@dateIssued", MySqlDbType = MySqlDbType.Date, Value = dto.DateIssued }
                   }.ToArray());
        }

        public void ReturnBook(ReturnBookDTO dto)
        {
            var sql = "UPDATE rent_history SET ReturnDate=@returnDate WHERE RentId=@rentId";

            _dbHelper.ExecuteNonQuery(sql,
               new List<MySqlParameter> {
                   new MySqlParameter { ParameterName = "@rentId", MySqlDbType = MySqlDbType.Guid, Value = dto.RentId},
                   new MySqlParameter { ParameterName = "@dateIssued", MySqlDbType = MySqlDbType.Date, Value = dto.ReturnDate }
                  }.ToArray());
        }

        public Guid? GetBookId(Guid rentId)
        {
            var sql = "SELECT BookId FROM rent_history WHERE RentId=@rentId";

            var result = _dbHelper.ExecuteScalarWithObjReturn(sql, new List<MySqlParameter> {
                new MySqlParameter { ParameterName = "@rentId", MySqlDbType = MySqlDbType.Guid, Value = rentId }
            }.ToArray());

            if (result == null)
            {
                return null;
            }
            else
            {
                return Guid.Parse(result.ToString());
            }
        }
    }
}
