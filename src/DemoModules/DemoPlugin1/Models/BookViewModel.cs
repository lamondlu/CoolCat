using System;

namespace DemoPlugin1.Models
{
    public class BookViewModel
    {
        public Guid BookId { get; set; }

        public string BookName { get; set; }

        public string ISBN { get; set; }

        public DateTime DateIssued { get; set; }
    }
}
