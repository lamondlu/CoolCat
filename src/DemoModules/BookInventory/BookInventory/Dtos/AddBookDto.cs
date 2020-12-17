using System;
using System.ComponentModel.DataAnnotations;

namespace BookInventory.Dtos
{
    public class AddBookDto
    {
        [Required]
        [Display(Name = "Book Name")]
        public string BookName { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        [Display(Name = "Date Issued")]
        public DateTime DateIssued { get; set; }

        public string Description { get; set; }
    }
}
