using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        public DbSet<Bid> Bids { get; set; } = null!;
        public DbSet<GoodsReceived> GoodsReceived { get; set; } = null!;
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; } = null!;
        public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;
        public DbSet<Auction> Auctions { get; set; } = null!;
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
        public DbSet<PaymentTransfer> PaymentTransfers { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Auction relationship
            modelBuilder.Entity<Auction>()
                .HasOne(a => a.PurchaseRequest)
                .WithMany()
                .HasForeignKey(a => a.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PurchaseOrder relationships
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.PurchaseRequest)
                .WithMany()
                .HasForeignKey(po => po.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Bid)
                .WithMany()
                .HasForeignKey(po => po.BidId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.OrderedByUser)
                .WithMany()
                .HasForeignKey(po => po.OrderedBy)
                .OnDelete(DeleteBehavior.Restrict);

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

            // Configure unique constraints
            modelBuilder.Entity<SupplierBranch>()
                .HasIndex(sb => new { sb.SupplierId, sb.BranchName })
                .IsUnique();

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.ItemName)
                .IsUnique();
        }
    }
}
