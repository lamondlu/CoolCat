using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Dtos
{
    public class RentBookDTO
    {
        public Guid BookId { get; set; }

        public string BookName { get; set; }

        public DateTime RentDate { get; set; }
    }
}
