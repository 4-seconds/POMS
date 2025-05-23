using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Purchase_Order_Management_System.Models;
using PurchaseOrderManagementSystem.Models;
using static Enum;
namespace PurchaseOrderManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<SupplierBranch> SupplierBranches { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Bid> Bids { get; set; } = null!;
        public DbSet<GoodsReceived> GoodsReceived { get; set; } = null!;
        public DbSet<PurchaseRequestOrder> PurchaseRequests { get; set; } = null!;
        public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Supplier)
                .WithMany(s => s.Bids)
                .HasForeignKey(b => b.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bid>()
                .HasMany(b => b.GoodsReceived)
                .WithOne(gr => gr.Bid)
                .HasForeignKey(gr => gr.BidId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupplierBranch>()
                .HasOne(sb => sb.Supplier)
                .WithMany(s => s.Branches)
                .HasForeignKey(sb => sb.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure unique constraints
            modelBuilder.Entity<SupplierBranch>()
                .HasIndex(sb => new { sb.SupplierId, sb.BranchName })
                .IsUnique();

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.ItemName)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            // Seed Admin Role
            var adminRoleId = Guid.NewGuid().ToString();
            var adminRole = new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            modelBuilder.Entity<IdentityRole>().HasData(adminRole);

            // Seed Admin User
            var adminUserId = Guid.NewGuid().ToString();
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                Address = "123 Admin Street",
                Gender = Gender.Male,
                AccountStatus = AccountStatus.Active
            };


            var passwordHasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);


            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            });
        }

    }
}
