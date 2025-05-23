using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ClothingAPIs.Models
{
	public class Product
	{
		[Key]
		public int Code { get; set; }
		public Color color { get; set; } 
		public string Name { get; set; }
		public string Description { get; set; }
		public double Price { get; set; }
		public string ImageUrl { get; set; }
		public List<ProductAdditionalImage> productAdditionalImages { get; set; }
		public int CategoryId { get; set; }
		public Category Category { get; set; }

		[ForeignKey("SubCategory")]
		public int SubCategoryId { get; set; }
		public SubCategory SubCategory { get; set; }

		public int Stock { set; get; }
		public Size size { get; set; }

		//public List<Order> orders { set; get; }
		public List<OrderItem>? OrderItems { set; get; }
		public List<Review>? Reviews { set; get; }
		public List<WishList>? WishLists { set; get; }

	}
	public enum Size
	{
		XS, S, M, L, XL, XXL
	}
	public enum Color
	{
		Black = 0,
		Blue = 1,
		Green = 2,
		Red = 3,
		Grey = 4,
		Yellow = 5,
		White = 6
	}
}
