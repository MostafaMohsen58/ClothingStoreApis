namespace ClothingAPIs.Models
{
	public class Category
	{
		public int id { set; get; }
		public string Name { set; get; }
		public List<Product>? Products { set; get; }
		public List<SubCategory>? SubCategories { set; get; }

	}
}
