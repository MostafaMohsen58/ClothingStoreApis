using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClothingAPIs.Models
{
	public class ApplicationDbContext:IdentityDbContext<AppUser>
	{
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<WishList> WishLists { get; set; }
		public DbSet<SubCategory> SubCategories { set; get; }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AppUser>()
				.HasMany(a => a.PastOrders)
				.WithOne(o => o.User)
				.HasForeignKey(o => o.UserId)
				.OnDelete(DeleteBehavior.Cascade); // Ensures orders are deleted if user is deleted

			modelBuilder.Entity<AppUser>()
				.HasOne(a => a.Cart)
				.WithOne(c => c.CartUser)
				.HasForeignKey<AppUser>(a => a.CartId)
				.OnDelete(DeleteBehavior.NoAction); // Ensures Cart is not deleted when Order is deleted

			base.OnModelCreating(modelBuilder);
		}

	}
}
