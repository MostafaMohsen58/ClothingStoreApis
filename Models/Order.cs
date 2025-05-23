using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingAPIs.Models
{
	public class Order
	{
		public int Id { get; set; }
		public DateTime OrderDate { get; set; }
		//public decimal TotalAmount { get; set; }
		[NotMapped]
		public double TotalAmount => OrderItems.Sum(oi => oi.Product.Price * oi.quantity);
		public PaymentMethod method { set; get; }
		public string? BillignAddress { set; get; }
		public string? AdditionalNotes { set; get; }
		public string? stripedetails { set; get; }
		public List<OrderItem>? OrderItems { get; set; }
		//[ForeignKey("User")]
		public string? UserId { set; get; }
		[InverseProperty("PastOrders")]
		public AppUser? User { set; get; }

		[InverseProperty("Cart")]
		public AppUser? CartUser { set; get; }
	}
	public enum PaymentMethod
	{
		OnDelivery,
		CreditCard,
		Stripe,
	}
}
