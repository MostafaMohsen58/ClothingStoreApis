using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingAPIs.Models
{
	public class AppUser : IdentityUser
	{
		public string FirstName { set; get; }
		public string LastName { set; get; }
		public DateOnly DateOfBirth { set; get; }
		public DateOnly CreatedAt { set; get; }
		[InverseProperty("User")]
		public List<Order>? PastOrders { set; get; }
		//[ForeignKey("Cart")]
		public int CartId { set; get; }
		[InverseProperty("CartUser")]
		public Order Cart { set; get; } = new Order();

	}

}
