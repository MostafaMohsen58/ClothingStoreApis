using ClothingAPIs.IRepoServices;
using ClothingAPIs.Models;
using ClothingAPIs.DTO;
using Microsoft.EntityFrameworkCore;

namespace ClothingAPIs.RepoServices
{
	public class WishListService(ApplicationDbContext context) : IWishListService
	{
		public void Create(int ProdId, string userId)
		{
			if (!GetWishListByUserId(userId).Contains(ProdId))
			{
				WishList wishlist = new()
				{
					ProductId = ProdId,
					UserId = userId
				};
				context.WishLists.Add(wishlist);
				context.SaveChanges();
			}
		}
		public void DeleteWishList(int ProdId, string userId)
		{
			var wishlist = context.WishLists.FirstOrDefault(wl => wl.UserId == userId && wl.ProductId == ProdId);
			if (wishlist != null)
			{
				context.WishLists.Remove(wishlist);
				context.SaveChanges();
			}


		}

		public List<int> GetWishListByUserId(string userId)
		{

			var userFavs = context.WishLists.Where(wl => wl.UserId == userId).Select(wl => wl.ProductId).ToList();
			return userFavs;
		}
	}
}
