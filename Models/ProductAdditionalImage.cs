namespace ClothingAPIs.Models
{
	public class ProductAdditionalImage
	{
		public int id { set; get; }
		public string img { set; get; }
		public int ProductId { set; get; }
		public Product product { set; get; }
	}
}
