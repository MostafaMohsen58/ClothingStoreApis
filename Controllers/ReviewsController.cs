using ClothingAPIs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ClothingAPIs.DTO;

namespace ClothingAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ReviewsController : ControllerBase
    {


        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsByProductId(int productId)
        {
            
            var reviews = await _context.Reviews
                .Include(R=>R.Product).Include(r=>r.User)
                .Where(r => r.ProductId == productId)
                .ToListAsync();         

            if (!reviews.Any())
            {
                return NotFound(); 
            }


		

			return Ok(reviews.Select(r => new { r.Id, ismine = User is null ? false : User.FindFirstValue(ClaimTypes.NameIdentifier) == r.UserId ? true : false, r.User.UserName, Productname = r.Product.Name, productId, r.UserId, r.ReviewDate, r.Comment, r.Rating }).ToList());
		}

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Review>> PostReview(ReviewDTO reviewdto)
        {
         

            var review = new Review()
            {
                ReviewDate = DateTime.Now,
                 Comment= reviewdto.Comment,
                  Rating= reviewdto.Rating,
                   ProductId= reviewdto.ProductId,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
			};


            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

			var reviews = await _context.Reviews
	         .Include(R => R.Product).Include(r => r.User)
	         .Where(r => r.ProductId == review.ProductId)
	         .ToListAsync();

			return Ok(reviews.Select(r => new { ismine = User is null ? false : User.FindFirstValue(ClaimTypes.NameIdentifier) == r.UserId ? true : false, r.User.UserName, Productname = r.Product.Name, reviewdto.ProductId, r.UserId, r.ReviewDate, r.Comment, r.Rating, r.Id }).ToList());


		}

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            if(review.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
			{
				return Unauthorized();
			}
			_context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

			var reviews = await _context.Reviews
             .ToListAsync();

			return Ok(reviews.Select(r => new { r.Id }));
		}
    }
}
