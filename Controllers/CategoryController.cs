using ClothingAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClothingAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
		[Authorize]

		public IActionResult Get()
        {
            return Ok(context.Categories
                            .Include(c => c.Products)
                            .Select(c => new { c.id, c.Name, Products = c.Products.Select(p => new {p.Name, p.Code, p.Price, p.color, p.size,p.Description,p.Reviews,p.Stock}) })
                            .AsNoTracking()
                            .ToList());
        }
		[HttpGet("GetSubCategories")]
		public IActionResult GetSubCategories()
		{
			var subCategories = context.SubCategories.Select(sc => new
			{
				sc.Id,
				sc.Name,
				sc.CategoryId,
				CategoryName = sc.Category.Name,
				ProductCount = sc.Products.Count()
			}).ToList();

			return Ok(subCategories);
		}

	}
}
