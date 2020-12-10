using System;

namespace BookLibrary.Dtos
{
    public class ReturnBookDTO
    {
        public Guid RentId { get; set; }

        public DateTime ReturnDate { get; set; }
    }
}
