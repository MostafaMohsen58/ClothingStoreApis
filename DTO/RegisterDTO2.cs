using System.ComponentModel.DataAnnotations;

namespace ClothingAPIs.DTO
{
	public class RegisterDTO2
	{
		public string id { set; get; }
		public string FirstName { set; get; }
		public string LastName { set; get; }
		public string UserName { set; get; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

		public DateOnly DateOfBirth { set; get; }
		public string Email { set; get; }
		[Phone]
		[MaxLength(11)]
		[MinLength(11)]
		public string PhoneNumber { set; get; }
		[DataType(DataType.Password)]
		public string? Password { set; get; }
		[Compare("Password")]
		[DataType(DataType.Password)]
		public string? ConfirmPassword { set; get; }

		public bool? rememberme { set; get; } = false;
	}
}
