using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoolCat
{
    public class LoginDTO
    {
        [DisplayName("User Name")]
        [Required]
        public string UserName { get; set; }

        [DisplayName("User Name")]
        [Required]
        public string Password { get; set; }
    }
}
