using System.ComponentModel.DataAnnotations;

namespace HoldEvent.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public String UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public String Password { get; set; }
    }
}
