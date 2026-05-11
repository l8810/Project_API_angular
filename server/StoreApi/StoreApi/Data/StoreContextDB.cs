using Microsoft.EntityFrameworkCore;
using StoreApi.Models;

namespace StoreApi.Data
{
    public class StoreContextDB : DbContext
    {
        public StoreContextDB(DbContextOptions<StoreContextDB> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<ProductCart> ProductCarts { get; set; }
        public DbSet<ProductPurchased> ProductPurchaseds { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public IEnumerable<object> Purchaseds { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User <-> ProductCart (one-to-many)
            modelBuilder.Entity<ProductCart>()
                .HasOne(pc => pc.user)          // navigation on ProductCart (property name in your model)
                .WithMany(u => u.Cart)          // navigation on User (collection name in your model)
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductCart -> Gift (many carts may reference the same Gift)
            modelBuilder.Entity<ProductCart>()
                .HasOne(pc => pc.Gift)
                .WithMany()                     // no inverse collection on Gift in current model
                .HasForeignKey(pc => pc.GiftId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductPurchased -> Gift
            modelBuilder.Entity<ProductPurchased>()
                .HasOne(pp => pp.Gift)
                .WithMany(g => g.Purchaseds)    // uses Gift.Purchaseds collection
                .HasForeignKey(pp => pp.GiftId)
                .OnDelete(DeleteBehavior.Restrict);

            ////ProductPurchased->User
            //modelBuilder.Entity<ProductPurchased>()
            //    .HasOne(pp => pp.User)
            //    .WithMany()        // uses User.orders collection
            //    .HasForeignKey(pp => pp.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // Gift -> Category (many gifts per category)
            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Category)
                .WithMany(c => c.Gifts)
                .HasForeignKey(g => g.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Gift -> Donor (many gifts per donor)
            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Donor)
                .WithMany(d => d.Gifts)
                .HasForeignKey(g => g.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure indexes / common constraints for performance and safety
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(false); // change to true if emails must be unique

            // Ensure collection navigation properties are ignored if missing in models
            // (No-op here; left as a place to extend if you change model property names.)
        }
    }
}