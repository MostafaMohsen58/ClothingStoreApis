using ClothingAPIs.DTO;

namespace ClothingAPIs.Models
{

		public class StripeSettings
		{
			public string SecretKey { get; set; }
			public string PublishableKey { get; set; }
		}
		public class PaymentResponse
		{
			public string Url { get; set; }
			public string SessionId { get; set; }
			public string PublicKey { get; set; }
		}
		public class CheckoutRequest
		{
			public string Amount { get; set; }
		}
}
