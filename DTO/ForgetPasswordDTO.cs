using System.ComponentModel.DataAnnotations;

namespace ClothingAPIs.DTO
{
    public class ForgetPasswordDTO
    {
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; }
    }
}
