

using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationApplicationUser> NotificationApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationApplicationUser>()
                .HasKey(t => new { t.NotificationId, t.ApplicationUserId });

            modelBuilder.Entity<NotificationApplicationUser>()
                .HasOne(pt => pt.Notification)
                .WithMany(p => p.NotificationApplicationUsers)
                .HasForeignKey(pt => pt.NotificationId);

            modelBuilder.Entity<NotificationApplicationUser>()
                .HasOne(pt => pt.ApplicationUser)
                .WithMany(t => t.NotificationApplicationUsers)
                .HasForeignKey(pt => pt.ApplicationUserId);

            base.OnModelCreating(modelBuilder);
        }
    }

}
