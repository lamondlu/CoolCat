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

        public void RentBook()
        {

        }
    }
}
