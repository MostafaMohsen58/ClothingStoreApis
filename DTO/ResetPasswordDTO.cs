using System.ComponentModel.DataAnnotations;

namespace ClothingAPIs.DTO
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match the new password")]
        public string ConfirmNewPassword { get; set; }
    }
}
