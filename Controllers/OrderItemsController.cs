using ClothingAPIs.DTO;
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
	public class OrderItemsController(ApplicationDbContext _context, UserManager<AppUser> userManager) : ControllerBase
	{
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Post(AddToCartDto dto)
		{
			var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
			if (user == null)
			{
				return NotFound();
			}
			var cart = _context.Orders.Include(o => o.OrderItems).FirstOrDefault(c => c.Id == user.CartId);
			if (cart == null)
			{
				var newCart = new Order
				{
					OrderDate = DateTime.UtcNow,
					//TotalAmount = 0,
					OrderItems = new List<OrderItem>()
				};
				_context.Orders.Add(newCart);
				user.CartId = newCart.Id;
				_context.SaveChanges();
			}


			var product = _context.Products.Find(dto.ProductId);
			if(product is null)
			{
				return BadRequest("Invalid product");
			}
			if (product.Stock >= dto.Quantity)
			{
				if (cart.OrderItems.Any(oi => oi.ProductId == product.Code))
				{
					var myorderitem = cart.OrderItems.FirstOrDefault(oi => oi.ProductId == product.Code);
					if (myorderitem.quantity + dto.Quantity > product.Stock)
					{
						return BadRequest("No stock");
					}
					else
					{
						myorderitem.quantity += dto.Quantity;
						_context.OrderItems.Update(myorderitem);
					}
				}
				else
				{
					var orderitem = new OrderItem()
					{
						ProductId = dto.ProductId,
						OrderId = user.CartId,
						quantity = dto.Quantity
					};
					//product.Stock -= dto.Quantity;

					_context.OrderItems.Add(orderitem);
				}
				//_context.Update(product);
				_context.SaveChanges();
				return Ok(new { product.Name });
			}
			else
			{
				return BadRequest("No stock");
			}
		}


		[HttpDelete("{OrderItemId}")]
		[Authorize]
		public async Task<IActionResult> Delete(int OrderItemId)
		{

			var orderitem = _context.OrderItems.Include(oi => oi.Order).ThenInclude(o => o.User).Include(o => o.Order).ThenInclude(oi => oi.CartUser)
				.FirstOrDefault(oi => oi.Id == OrderItemId);

			if (orderitem.Order.CartUser is not null)
			{
				if (orderitem.Order.CartUser.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
				{

					_context.OrderItems.Remove(orderitem);
					_context.SaveChanges();
					return Ok();

				}
				else
				{
					return BadRequest("You can't Delete from that cart");
				}
			}
			else
			{
				return BadRequest("The order is confirmed and can't be cancelled");
			}

		}

		[HttpPut("Increase/{OrderItemId}")]
		[Authorize]
		public async Task<IActionResult> Increase(int OrderItemId)
		{

			var orderitem = _context.OrderItems.Include(oi => oi.Order).ThenInclude(o => o.User).Include(o => o.Order).ThenInclude(oi => oi.CartUser)
				.FirstOrDefault(oi => oi.Id == OrderItemId);

			if (orderitem.Order.CartUser is not null)
			{
				if (orderitem.Order.CartUser.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
				{
					if (orderitem.quantity + 1 <= _context.Products.Find(orderitem.ProductId).Stock)
					{
						orderitem.quantity++;
						_context.OrderItems.Update(orderitem);
						_context.SaveChanges();
						return Ok(
							_context.Orders.Include(o => o.OrderItems)
							 .ThenInclude(o => o.Product)
							 .Include(o => o.User)
							 .Select(o => new
							 {
								 o.Id,
								 o.OrderDate,
								 o.TotalAmount,
								 OrderItems = o.OrderItems.Select(oi => new { oi.Id, oi.Product.Name, oi.Product.ImageUrl, oi.quantity, oi.Product.Price }).ToArray(),

							 }).FirstOrDefault(c => c.Id == orderitem.OrderId)

							);
					}
					else
					{
						return BadRequest("No Stock");
					}
				}
				else
				{
					return BadRequest("You can't Delete from that cart");
				}
			}
			else
			{
				return BadRequest("The order is confirmed and can't be cancelled");
			}

		}


		[HttpPut("Decrease/{OrderItemId}")]
		[Authorize]
		public async Task<IActionResult> Decrease(int OrderItemId)
		{

			var orderitem = _context.OrderItems.Include(oi => oi.Order).ThenInclude(o => o.User).Include(o => o.Order).ThenInclude(oi => oi.CartUser)
				.FirstOrDefault(oi => oi.Id == OrderItemId);
			if (orderitem.Order.CartUser is not null)
			{
				if (orderitem.Order.CartUser.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
				{
					if (orderitem.quantity > 1)
					{
						orderitem.quantity--;
						_context.OrderItems.Update(orderitem);
						_context.SaveChanges();
						return Ok(
								_context.Orders.Include(o => o.OrderItems)
								 .ThenInclude(o => o.Product)
								 .Include(o => o.User)
								 .Select(o => new
								 {
									 o.Id,
									 o.OrderDate,
									 o.TotalAmount,
									 OrderItems = o.OrderItems.Select(oi => new { oi.Id, oi.Product.Name, oi.Product.ImageUrl, oi.quantity, oi.Product.Price }).ToArray(),

								 }).FirstOrDefault(c => c.Id == orderitem.OrderId)

								);
					}
					else
					{
						return BadRequest();

					}
				}
				else
				{
					return BadRequest("You can't Delete from that cart");
				}
			}
			else
			{
				return BadRequest("The order is confirmed and can't be cancelled");
			}

		}
	}
}
//asdasd
