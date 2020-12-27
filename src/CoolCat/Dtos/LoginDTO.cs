using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
