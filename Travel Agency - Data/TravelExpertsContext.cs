using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Travel_Agency___Data.Models
{
    public partial class TravelExpertsContext : IdentityDbContext<User>
    {
        public TravelExpertsContext(DbContextOptions<TravelExpertsContext> options)
            : base(options)
        {
        }

        // DbSet properties for all your tables
        public virtual DbSet<Affiliation> Affiliations { get; set; }
        public virtual DbSet<Agency> Agencies { get; set; }
        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingDetail> BookingDetails { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<CreditCard> CreditCards { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomersReward> CustomersRewards { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Fee> Fees { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<PackagesProductsSupplier> PackagesProductsSuppliers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductsSupplier> ProductsSuppliers { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Reward> Rewards { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SupplierContact> SupplierContacts { get; set; }
        public virtual DbSet<TripType> TripTypes { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; } // Added Wallet DbSet
        public virtual DbSet<Purchase> Purchases { get; set; }// Added Purchase DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required for Identity integration.

            // Affiliation entity configuration
            modelBuilder.Entity<Affiliation>(entity =>
            {
                entity.HasKey(e => e.AffilitationId)
                    .HasName("aaaaaAffiliations_PK")
                    .IsClustered(false);

                entity.Property(e => e.AffilitationId)
                    .HasMaxLength(10);

                entity.Property(e => e.AffDesc)
                    .HasMaxLength(50);

                entity.Property(e => e.AffName)
                    .HasMaxLength(50);
            });

            // Additional entity configurations can go here...

            // Wallet entity configuration
            // Wallet entity configuration
            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(e => e.WalletId); // Primary key

                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(18,2)") // Define precision for decimal values
                    .IsRequired();

                // Ensure the foreign key relationship is correct
                entity.HasOne<Customer>() // Foreign key relationship with Customer
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder); // For partial method implementation
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
