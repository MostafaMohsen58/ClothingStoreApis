using System.ComponentModel.DataAnnotations;

namespace ClothingAPIs.DTO
{
    public class LoginDTO
    {
        public string UsernameOrEmail { set; get; }
        [DataType(DataType.Password)]
        public string Password { set; get; }
        public bool RememberMe { set; get; }
    }
}
