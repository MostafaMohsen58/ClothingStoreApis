using ClothingAPIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClothingAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(ApplicationDbContext context) : ControllerBase
    {
		[HttpGet]
		public IActionResult Get()
		{
			var products = context.Products.Include(p=>p.OrderItems).AsNoTracking().Select(p => new
			{
				p.Code,
				p.Name,
				p.Description,
				p.Price,
				p.ImageUrl,
				color = (int)p.color,
				size = (int)p.size,
				categoryid = p.CategoryId,
				subcategoryid = p.SubCategoryId,
				orderItems = p.OrderItems,
				p.Stock
			});

			return Ok(products);
		}

		[HttpGet("{code}")]
        public IActionResult Get(int code)
        {
			int x = 0;
			var ret = context.Products.Include(p => p.Reviews)
										.ThenInclude(p => p.User)
										.Include(p => p.Category)
										.Include(p => p.productAdditionalImages)
										.Select(p => new { p.Stock,p.Code, p.Name, p.Description, p.Price, p.size, p.color, p.ImageUrl, Comments = p.Reviews.Select(r => new { r.Comment, r.ReviewDate, r.User.UserName, r.Rating }), Category = p.Category.Name, images = p.productAdditionalImages.Select(i => i.img) })
										.FirstOrDefault(p => p.Code == code);

			return Ok(ret);
        }


        //price, colour, rating, category, size
        [HttpGet("GetFiltered")]
        public IActionResult GetFiltered(double MinPrice=0, double MaxPrice=0, Color color= (Color)16, double rating=0, int CatId=0, Size size=(Size) 7 )
        {
            return Ok(context.Products.Include(p => p.Reviews)
                                        .ThenInclude(p => p.User)
                                        .Include(p => p.Category)
                                        .Select(p => new { p.Code, p.Name, p.Description,p.size, p.Price, p.ImageUrl, p.color, Comments = p.Reviews.Select(r => new { r.Comment, r.ReviewDate, r.User.UserName, r.Rating }), CategoryName = p.Category.Name, p.CategoryId })
                                        .Where(p => (p.Price > MinPrice && p.Price < MaxPrice) || MaxPrice == 0)
                                        .Where(p => p.color == color || color == (Color)16)
                                        .Where(p => p.Comments.Sum(c => c.Rating) / p.Comments.Count() >= rating || rating == 0)
                                        .Where(p => p.CategoryId == CatId || CatId == 0)
                                        .Where(p=>p.size==size || size == (Size)7)
                                        .ToList()); 


        }



		[HttpGet("Categories")]
		public IActionResult GetCategories()
		{
			var categories = context.Categories.Include(c => c.SubCategories)
												.Select(c => new { c.id, c.Name /*, SubCategories = c.SubCategories.Select(s => new { s.Id, s.Name }) */ })
												.ToList();
			return Ok(categories);
		}

		[HttpGet("SubCategories")]
		public IActionResult GetSubCategories()
		{
			var subcategories = context.SubCategories.Select(s => new { s.Id, s.Name, CategoryId = s.CategoryId }).ToList();
			return Ok(subcategories);
		}


		[HttpGet("GetSubCategoryByCatId/{id}")]
		public IActionResult GetSubCategoryByCatId(int id)
		{
			var result = context.SubCategories.Where(c => c.CategoryId == id).Select(s => new { s.Id, s.Name }).ToList();
			return Ok(result);
		}

	}
}
