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

        public bool CheckBookTracking(Guid bookId)
        {
            var sql = "SELECT COUNT(*) FROM Book WHERE BookId=@bookId";

            var count = _dbHelper.ExecuteScalar(sql);

            return count == 1;
        }

        public void RentBook()
        {
            var sql = "";
        }

        public void RentBookWithNew()
        {
            var sql = "";
        }
    }
}
