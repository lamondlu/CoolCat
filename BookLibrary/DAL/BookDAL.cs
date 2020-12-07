using BookLibrary.Dtos;
using MySql.Data.MySqlClient;
using Mystique.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var sql = "INSERT INTO rent_history(RentId, BookId, BookName, ISBN, RentDate) values(@rentId, @bookId, @bookName, @isbn, @rentDate)";

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
    }
}
