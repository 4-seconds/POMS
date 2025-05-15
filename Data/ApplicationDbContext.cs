using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Purchase_Order_Management_System.Models;
using PurchaseOrderManagementSystem.Models;

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
        }
    }
}
