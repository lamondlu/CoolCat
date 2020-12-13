using System;

namespace BookInventory.ViewModels
{
    public class BookListViewModel
    {
        public Guid BookId { get; set; }

        public string BookName { get; set; }

        public string ISBN { get; set; }

        public DateTime DateIssued { get; set; }

        public bool Status { get; set; }
    }
}
