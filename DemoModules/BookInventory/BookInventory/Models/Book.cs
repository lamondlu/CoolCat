using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookInventory.Models
{
    public class Book
    {
        public Guid BookId { get; set; }

        public string BookName { get; set; }

        public string ISBN { get; set; }

        public DateTime DateIssued { get; set; }

        public string Description { get; set; }
    }
}
