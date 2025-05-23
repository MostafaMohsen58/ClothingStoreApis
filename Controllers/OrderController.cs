using ClothingAPIs.DTO;
using ClothingAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using Stripe.Checkout;
using Stripe;
using Stripe.V2;

namespace ClothingAPIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController(ApplicationDbContext _context, UserManager<AppUser> userManager, StripeSettings _stripesettings, ILogger<OrderController> _logger) : ControllerBase
	{


		[HttpGet("All")]
		[Authorize]
		public IActionResult GetAllOrders()
		{
			var ret = _context.Orders.Include(o => o.OrderItems)
				.ThenInclude(o => o.Product)
				.Include(o => o.User)
				.Select(o => new
				{
					o.Id,
					o.OrderDate,
					o.TotalAmount,
					o.method,
					o.BillignAddress,
					o.stripedetails,
					OrderItems = o.OrderItems.Select(oi => new { oi.Id, oi.Product.Name, oi.Product.ImageUrl, oi.quantity, oi.Product.Price }).ToArray(),

				})
				.ToList();

			return Ok(ret);
		}


		// GET: api/<OrderController>
		[HttpGet]
		[Authorize]
		public IActionResult Get()
		{
			var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var ret = _context.Orders.Include(o => o.OrderItems)
				.ThenInclude(o => o.Product)
				.Include(o => o.User)
				//.Where(o => o.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && o.CartUser!=null)
				.Where(o => o.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
				.Select(o => new
				{
					o.Id,
					o.OrderDate,
					o.TotalAmount,
					o.method,
					o.BillignAddress,
					o.stripedetails,
					OrderItems = o.OrderItems.Select(oi => new { oi.Id, oi.Product.Name, oi.Product.ImageUrl, oi.quantity, oi.Product.Price }).ToArray(),

				})
				.ToList();

			return Ok(ret);
		}

		// GET api/<OrderController>/5
		[HttpGet("{id}")]
		[Authorize]
		public IActionResult Get(int id)
		{

			var order = _context.Orders.Include(o => o.OrderItems)
			   .ThenInclude(o => o.Product)
			   .Include(o => o.User)
			   .Select(o => new
			   {
				   o.Id,
				   o.OrderDate,
				   o.TotalAmount,
				   OrderItems = o.OrderItems.Select(oi => new { oi.Product.Name, oi.Product.ImageUrl, oi.quantity, oi.Product.Price }).ToArray(),


			   }).FirstOrDefault(o => o.Id == id);

			if (order == null)
			{
				return NotFound();
			}

			return Ok(order);

		}
		[HttpGet("GetCart")]
		[Authorize]
		public async Task<IActionResult> GetCart()
		{

			var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var order = _context.Orders.Include(o => o.OrderItems)
			   .ThenInclude(o => o.Product)
			   .Include(o => o.User)
			   .Select(o => new
			   {
				   o.Id,
				   o.OrderDate,
				   o.TotalAmount,
				   OrderItems = o.OrderItems.Select(oi => new { oi.Id, oi.Product.Name, oi.Product.ImageUrl, oi.quantity, oi.Product.Price }).ToArray(),

			   }).FirstOrDefault(o => o.Id == user.CartId);

			if (order == null)
			{
				return NotFound();
			}

			return Ok(order);

		}
		[HttpPut("EmptyCart")]
		[Authorize]
		public async Task<IActionResult> EmptyCart()
		{
			var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

			if (user == null)
			{
				return NotFound();
			}
			var cart = _context.Orders.Include(o => o.OrderItems).FirstOrDefault(c => c.Id == user.CartId);



			foreach (var item in cart.OrderItems)
			{
				_context.OrderItems.Remove(item);
			}
			_context.SaveChanges();
			return Ok(_context.Orders.Include(o => o.OrderItems)
			   .ThenInclude(o => o.Product)
			   .Include(o => o.User)
			   .Select(o => new
			   {
				   o.Id,
				   o.OrderDate,
				   o.TotalAmount,
				   OrderItems = o.OrderItems.Select(oi => new { oi.Id, oi.Product.Name, oi.Product.ImageUrl, oi.quantity, oi.Product.Price }).ToArray(),

			   }).FirstOrDefault(o => o.Id == user.CartId));
		}

		//// POST api/<OrderController>
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Post(BillingDto dto, string successurl = "", bool Stripesucceeded = false,string sessionid=null)
		{
			
			var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
			if (user == null)
			{
				return NotFound();
			}
			var cart = _context.Orders.Include(o => o.OrderItems).ThenInclude(oi=>oi.Product).FirstOrDefault(c => c.Id == user.CartId);
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
			if (dto.method == Models.PaymentMethod.Stripe && !Stripesucceeded)
			{
				HttpContext.Session.SetString("BillingDto", JsonConvert.SerializeObject(dto));
				//var client = _httpClientFactory.CreateClient();
				//var result=client.PostAsJsonAsync
				return RedirectToAction("CreateCheckoutSession", "Order", new { Amount = cart.TotalAmount, successurl = successurl });
			}
			else if(Stripesucceeded)
			{
				string serializedDto = HttpContext.Session.GetString("BillingDto");
				if (!string.IsNullOrEmpty(serializedDto))
				{
					dto = JsonConvert.DeserializeObject<BillingDto>(serializedDto);  // Deserialize back into BillingDto
				}
				else
				{
					return BadRequest("Billing details are missing.");
				}
			}


			if (cart.OrderItems.Count() != 0)
			{
				foreach (var orderitem in cart.OrderItems)
				{
					var product = _context.Products.Find(orderitem.ProductId);
					if (orderitem.quantity > product.Stock)
					{
						return BadRequest($"no stock for product {product.Name}");
					}

				}

				foreach (var orderitem in cart.OrderItems)
				{
					var product = _context.Products.Find(orderitem.ProductId);
					product.Stock -= orderitem.quantity;
					_context.Update(product);
				}
				user.Cart.UserId = user.Id;
				user.Cart.BillignAddress = $"{dto.StreetAddress},{dto.City},{dto.State},{dto.Country},{dto.Postcode}";
				user.Cart.AdditionalNotes = dto.OrderNotes != null ? dto.OrderNotes : "";
				user.Cart.OrderDate = DateTime.Now;
				user.Cart.method = dto.method;
				if (dto.method == Models.PaymentMethod.Stripe&&sessionid!=null)
				{
					user.Cart.stripedetails = sessionid;
				}
				if(dto.method== Models.PaymentMethod.CreditCard)
				{
					user.Cart.stripedetails = dto.paymentdetails;
				}

					var newCart = new Order
				{
					OrderDate = DateTime.UtcNow,
					//TotalAmount = 0,
					OrderItems = new List<OrderItem>()
				};
				_context.Orders.Add(newCart);
				_context.SaveChanges();
				user.CartId = newCart.Id;
				await userManager.UpdateAsync(user);

				_context.SaveChanges();
			}
			return Ok();


		}

		[HttpGet("create-checkout-session")]
		public ActionResult<PaymentResponse> CreateCheckoutSession(string Amount, string successurl="")
		{





			try
			{
				var baseUrl = $"{Request.Scheme}://{Request.Host}";

				//var baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host;
				var currency = "usd"; // Currency code
									  //var successUrl = $"{Request.Scheme}://{Request.Host}/api/Order/PaymentSuccess?session_id={{CHECKOUT_SESSION_ID}}&successurl={{successurl}}";
									  //var cancelUrl = $"{Request.Scheme}://{Request.Host}/api/Order/canceled";
				var successUrl = $"{baseUrl}/api/Order/success?session_id={{CHECKOUT_SESSION_ID}}&successurl={successurl}";
				var cancelUrl = $"{baseUrl}/api/Order/canceled";
				StripeConfiguration.ApiKey = _stripesettings.SecretKey;

				var options = new SessionCreateOptions
				{
					PaymentMethodTypes = new List<string>
				{
					"card"
				},
					LineItems = new List<SessionLineItemOptions>
				{
					new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							Currency = currency,
							UnitAmount = Convert.ToInt32(Math.Ceiling(decimal.Parse(Amount))) * 100,  // Amount in smallest currency unit (e.g., cents)
                            ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = "Product Name",
								Description = "Product Description"
							}
						},
						Quantity = 1
					}
				},
					Mode = "payment",
					SuccessUrl = successUrl,
					CancelUrl = cancelUrl
				};

				var service = new Stripe.Checkout.SessionService();
				var session = service.Create(options);
				//var successUrl = $"{baseUrl}/Invoice/Create/{session.Id}";
				//$"{Request.Scheme}://{Request.Host}/api/Order/PaymentSuccess?session_id={{session.Id}}&successurl={{successurl}}"
				options.SuccessUrl = $"{Request.Scheme}://{Request.Host}/api/Order/success?session_id={session.Id}&successurl={successurl}";
				session.SuccessUrl = $"{Request.Scheme}://{Request.Host}/api/Order/success?session_id={session.Id}&successurl={successurl}";


				return Ok(new { session.Url });

			}
			catch (StripeException e)
			{
				_logger.LogError(e, "Stripe error");
				return BadRequest(new { error = e.StripeError.Message });
			}




		
		}


		[HttpGet("success")]
		public async Task<IActionResult> PaymentSuccess([FromQuery] string session_id, [FromQuery] string successurl)
		{





			#region t3bt
			var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
			if (user == null)
			{
				return NotFound();
			}
			var cart = _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefault(c => c.Id == user.CartId);
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
			BillingDto dto = new();
			
			
				string serializedDto = HttpContext.Session.GetString("BillingDto");
				if (!string.IsNullOrEmpty(serializedDto))
				{
					dto = JsonConvert.DeserializeObject<BillingDto>(serializedDto);  // Deserialize back into BillingDto
				}
				else
				{
					return BadRequest("Billing details are missing.");
				}
			


			if (cart.OrderItems.Count() != 0)
			{
				foreach (var orderitem in cart.OrderItems)
				{
					var product = _context.Products.Find(orderitem.ProductId);
					if (orderitem.quantity > product.Stock)
					{
						return BadRequest($"no stock for product {product.Name}");
					}

				}

				foreach (var orderitem in cart.OrderItems)
				{
					var product = _context.Products.Find(orderitem.ProductId);
					product.Stock -= orderitem.quantity;
					_context.Update(product);
				}
				user.Cart.UserId = user.Id;
				user.Cart.BillignAddress = $"{dto.StreetAddress},{dto.City},{dto.State},{dto.Country},{dto.Postcode}";
				user.Cart.AdditionalNotes = dto.OrderNotes != null ? dto.OrderNotes : "";
				user.Cart.OrderDate = DateTime.Now;
				user.Cart.method = dto.method;
				if (dto.method == Models.PaymentMethod.Stripe && session_id != null)
				{
					user.Cart.stripedetails = session_id;
				}

				var newCart = new Order
				{
					OrderDate = DateTime.UtcNow,
					//TotalAmount = 0,
					OrderItems = new List<OrderItem>()
				};
				_context.Orders.Add(newCart);
				_context.SaveChanges();
				user.CartId = newCart.Id;
				await userManager.UpdateAsync(user);

				_context.SaveChanges();
			}
			return Redirect(successurl);
			#endregion



			
		}



		[HttpGet("cancel")]
		[AllowAnonymous]
		public IActionResult Cancel(
										[FromQuery] string session_id,  // Stripe includes this automatically
										[FromQuery] string reason = "") // Optional: Stripe may provide cancellation reason
		{
			_logger.LogInformation($"Checkout canceled. Session: {session_id}, Reason: {reason}");

			// Optional: Fetch session details from Stripe
			var sessionService = new Stripe.Checkout.SessionService();
			var session = sessionService.Get(session_id);

			return Ok(new
			{
				Message = "Payment was canceled.",
				SessionId = session_id,
				Amount = session.AmountTotal / 100,
				Currency = session.Currency,
				CustomerEmail = session.CustomerDetails?.Email,
				// Frontend can use this to redirect or show UI
				RedirectUrl = "https://your-app.com/checkout/retry",
				reason = reason
			});
		}

	}
}
