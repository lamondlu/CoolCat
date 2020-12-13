using System;

namespace BookLibrary.Dtos
{
    public class RentBookDTO
    {
        public Guid BookId { get; set; }

        public string BookName { get; set; }

        public string ISBN { get; set; }

        public DateTime DateIssued { get; set; }

        public DateTime RentDate { get; set; }
    }
}
