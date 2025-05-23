using System.ComponentModel.DataAnnotations;

namespace ClothingAPIs.DTO
{
    public class ExternalLoginDTO
    {
            [Required(ErrorMessage = "Provider is required.")]
            public string Provider { get; set; }
        
    }
}
