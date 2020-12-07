using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Dtos
{
    public class ReturnBookDTO
    {
        public Guid RentId { get; set; }

        public DateTime ReturnDate { get; set; }
    }
}
