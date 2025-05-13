using System.ComponentModel.DataAnnotations;

namespace HoldEvent.Models
{
    public class ChangePasswordViewModel
    {
        public String AccountID { get; set; }
        [Required(ErrorMessage = "mat khau khong duoc de trong")]
        public String OldPasswordHash { get; set; }
        [Required(ErrorMessage = "mat khau khong duoc de trong")]
        public String? newPassword { get; set; }
        [Required(ErrorMessage = "mat khau khong duoc de trong")]
        public String? ConfirmNewPassword { get; set; }
    }
}
