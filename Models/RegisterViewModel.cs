using System.ComponentModel.DataAnnotations;

namespace HoldEvent.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is require")]
        public String FullName { get; set; }
        [Required(ErrorMessage = "Email is require")]
        public String Email { get; set; }
        [Required(ErrorMessage = "Phone Number is require")]
        public String PhoneNumber { get; set; }
        public String? Address { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public Boolean Gender { get; set; }
        [Required(ErrorMessage = "User Name is require")]
        public String UserName { get; set; }
        [Required(ErrorMessage = "Password is require")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        [Required(ErrorMessage = "ConfirmPassword is require")]
        [DataType(DataType.Password)]
        public String? ConfirmPassword { get; set; }
    }
}
