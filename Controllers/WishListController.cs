using ClothingAPIs.DTO;
using ClothingAPIs.IRepoServices;
using ClothingAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClothingAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	public class WishListController(IWishListService wishListService, UserManager<AppUser> userManager, ApplicationDbContext context) : ControllerBase
	{
		[HttpPost("create")]
		[Authorize]
		public IActionResult Create(AddWishListDTO prd)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (userId == null)
			{
				return Unauthorized();
			}
			if(context.Products.FirstOrDefault(p=>p.Code== prd.prodid) ==null)
			{
				return BadRequest("Product not found");
			}
			wishListService.Create(prd.prodid, userId);
			return Ok(new { message = "Product added to wishlist" });
		}


		[HttpDelete("Delete")]
		[Authorize]
		public IActionResult Delete(int productId)
		{
			var selectedwl = context.WishLists.FirstOrDefault(wl => wl.ProductId == productId);
			if (selectedwl != null)
			{
				if (selectedwl.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
					wishListService.DeleteWishList(productId, selectedwl.UserId);
				else
					return Unauthorized($"only user {User.FindFirstValue(ClaimTypes.Name)} can Delete this wishlist");
			}
			return Ok(new { message = "Product removed from wishlist" });
		}


		[HttpGet("GetFavoriteByUserId")]
		public IActionResult GetByUser()
		{

				var products = wishListService.GetWishListByUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));

				return Ok(products);
			
		}
	}
}
