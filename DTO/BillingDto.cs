using ClothingAPIs.Models;
using System.ComponentModel.DataAnnotations;

namespace ClothingAPIs.DTO
{
	public class BillingDto
	{
		[Required]
		[MinLength(3)]
		public string Country { get; set; }

		[Required]
		[MinLength(5)]
		public string StreetAddress { get; set; }

		public string? Apartment { get; set; } // optional

		[Required]
		[MinLength(2)]
		public string City { get; set; }

		[Required]
		[MinLength(2)]
		public string State { get; set; }

		[Required]
		[RegularExpression(@"^\d{5}$", ErrorMessage = "Postcode must be exactly 5 digits.")]
		public string Postcode { get; set; }

		[Required]
		public bool AddNote { get; set; }

		[MaxLength(500)]
		public string? OrderNotes { get; set; } // optional
		public PaymentMethod method { set; get; }
		public string? paymentdetails { set; get; }
		
	}
}
