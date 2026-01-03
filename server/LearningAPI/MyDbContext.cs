using LearningAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LearningAPI
{
    public class MyDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public MyDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _configuration.GetConnectionString("MyConnection");
            if (connectionString != null)
                optionsBuilder.UseMySQL(connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentTracking> PaymentTrackings { get; set; }
        public DbSet<GBLSession> GBLSessions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Tutorial> Tutorials { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionItem> PromotionItems { get; set; }
        public DbSet<CartHeader> CartHeaders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User to Staff (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Staff)
                .WithOne(s => s.User)
                .HasForeignKey<Staff>(s => s.UserId);

            // User to Customer (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId);

            // User to Cart (1:0..1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.CartItems)
                .WithOne(c => c.User)
                .HasForeignKey<CartItem>(c => c.UserId)
                .IsRequired(false);

            // User to Orders (1:many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .IsRequired(false);

            // User to GBL_Sessions (1:many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.GBLSessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .IsRequired(false);

            // Cart to GBLSession (1:0..1)
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Session)
                .WithOne(s => s.CartItems)
                .HasForeignKey<GBLSession>(s => s.CartId)
                .IsRequired(false);

            // Promotion.campaign_id -> Campaign.campaign_id (1:N)
            modelBuilder.Entity<Promotion>()
                .HasOne(p => p.Campaign)
                .WithMany(c => c.Promotions)
                .HasForeignKey(p => p.CampaignId)
                .IsRequired(false);

            // Promotion_item.promotion_id -> Promotion.promotion_id (1:N)
            modelBuilder.Entity<PromotionItem>()
                .HasOne(pi => pi.Promotion)
                .WithMany(p => p.PromotionItems)
                .HasForeignKey(pi => pi.PromotionId)
                .IsRequired(false); 

            // Promotion_item.product_id -> Product.product_id (1:N)
            modelBuilder.Entity<PromotionItem>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.PromotionItems)
                .HasForeignKey(pi => pi.ProductId)
                .IsRequired(false);

            // CartItem.CartId -> CartHeader.CartId (many:1)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.CartHeader)
                .WithMany(ch => ch.CartItems)
                .HasForeignKey(ci => ci.CartItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}
