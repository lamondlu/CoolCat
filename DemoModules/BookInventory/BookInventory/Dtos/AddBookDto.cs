using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookInventory.Dtos
{
    public class AddBookDto
    {
        [Required]
        public string BookName { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public DateTime DateIssued { get; set; }

        public string Description { get; set; }
        
    }
}
