using System;

namespace BookInventory.ViewModels
{
    public class BookDetailViewModel
    {
        public Guid BookId { get; set; }

        public string BookName { get; set; }

        public string ISBN { get; set; }

        public DateTime DateIssued { get; set; }

        public string Description { get; set; }
    }
}
