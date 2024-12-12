﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Travel_Agency___Data.Models
namespace Travel_Agency___Data;

public partial class TravelExpertsContext : IdentityDbContext<User> 
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
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-AABS9BD\\SQLEXPRESS;Initial Catalog=TravelExperts;Integrated Security=True; TrustServerCertificate=true");

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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
    modelBuilder.Entity<Affiliation>(entity =>
        {
            entity.HasKey(e => e.AffilitationId)
                .HasName("aaaaaAffiliations_PK")
                .IsClustered(false);
        });

            // Other Entities...
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasOne(d => d.Agency).WithMany(p => p.Agents).HasConstraintName("FK_Agents_Agencies");
        });


        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId)
                .HasName("aaaaaBookings_PK")
                .IsClustered(false);

            entity.Property(e => e.PackageId).HasDefaultValue(0);

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookings).HasConstraintName("Bookings_FK00");

            entity.HasOne(d => d.Package).WithMany(p => p.Bookings).HasConstraintName("Bookings_FK01");

            entity.HasOne(d => d.TripType).WithMany(p => p.Bookings).HasConstraintName("Bookings_FK02");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => e.BookingDetailId)
                .HasName("aaaaaBookingDetails_PK")
                .IsClustered(false);

            entity.Property(e => e.BookingId).HasDefaultValue(0);
            entity.Property(e => e.ProductSupplierId).HasDefaultValue(0);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails).HasConstraintName("FK_BookingDetails_Bookings");

            entity.HasOne(d => d.Class).WithMany(p => p.BookingDetails).HasConstraintName("FK_BookingDetails_Classes");

            entity.HasOne(d => d.Fee).WithMany(p => p.BookingDetails).HasConstraintName("FK_BookingDetails_Fees");

            entity.HasOne(d => d.ProductSupplier).WithMany(p => p.BookingDetails).HasConstraintName("FK_BookingDetails_Products_Suppliers");

            entity.HasOne(d => d.Region).WithMany(p => p.BookingDetails).HasConstraintName("FK_BookingDetails_Regions");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId)
                .HasName("aaaaaClasses_PK")
                .IsClustered(false);
        });

        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.HasKey(e => e.CreditCardId)
                .HasName("aaaaaCreditCards_PK")
                .IsClustered(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.CreditCards)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CreditCards_FK00");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId)
                .HasName("aaaaaCustomers_PK")
                .IsClustered(false);

            entity.HasOne(d => d.Agent).WithMany(p => p.Customers).HasConstraintName("FK_Customers_Agents");
        });

        modelBuilder.Entity<CustomersReward>(entity =>
        {
            entity.HasKey(e => new { e.CustomerId, e.RewardId })
                .HasName("aaaaaCustomers_Rewards_PK")
                .IsClustered(false);

            OnModelCreatingPartial(modelBuilder);
        }
            entity.HasOne(d => d.Customer).WithMany(p => p.CustomersRewards)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Customers_Rewards_FK00");

            entity.HasOne(d => d.Reward).WithMany(p => p.CustomersRewards)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Customers_Rewards_FK01");
        });

        modelBuilder.Entity<Fee>(entity =>
        {
            entity.HasKey(e => e.FeeId)
                .HasName("aaaaaFees_PK")
                .IsClustered(false);
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId)
                .HasName("aaaaaPackages_PK")
                .IsClustered(false);

            entity.Property(e => e.PkgAgencyCommission).HasDefaultValue(0m);
        });

        modelBuilder.Entity<PackagesProductsSupplier>(entity =>
        {
            entity.HasKey(e => e.PackageProductSupplierId).HasName("PK__Packages__53E8ED9938A61292");
            entity.HasKey(e => e.PackageProductSupplierId).HasName("PK__Packages__53E8ED99B8A6DCAD");

            entity.HasOne(d => d.Package).WithMany(p => p.PackagesProductsSuppliers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Packages_Products_Supplie_FK00");

            entity.HasOne(d => d.ProductSupplier).WithMany(p => p.PackagesProductsSuppliers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Packages_Products_Supplie_FK01");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId)
                .HasName("aaaaaProducts_PK")
                .IsClustered(false);
        });

        modelBuilder.Entity<ProductsSupplier>(entity =>
        {
            entity.HasKey(e => e.ProductSupplierId)
                .HasName("aaaaaProducts_Suppliers_PK")
                .IsClustered(false);

            entity.Property(e => e.ProductId).HasDefaultValue(0);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductsSuppliers).HasConstraintName("Products_Suppliers_FK00");

            entity.HasOne(d => d.Supplier).WithMany(p => p.ProductsSuppliers).HasConstraintName("Products_Suppliers_FK01");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId)
                .HasName("aaaaaRegions_PK")
                .IsClustered(false);
        });

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.HasKey(e => e.RewardId)
                .HasName("aaaaaRewards_PK")
                .IsClustered(false);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId)
                .HasName("aaaaaSuppliers_PK")
                .IsClustered(false);
        });

        modelBuilder.Entity<SupplierContact>(entity =>
        {
            entity.HasKey(e => e.SupplierContactId)
                .HasName("aaaaaSupplierContacts_PK")
                .IsClustered(false);

            entity.Property(e => e.SupplierContactId).ValueGeneratedNever();
            entity.Property(e => e.SupplierId).HasDefaultValue(0);

            entity.HasOne(d => d.Affiliation).WithMany(p => p.SupplierContacts).HasConstraintName("SupplierContacts_FK00");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierContacts).HasConstraintName("SupplierContacts_FK01");
        });

        modelBuilder.Entity<TripType>(entity =>
        {
            entity.HasKey(e => e.TripTypeId)
                .HasName("aaaaaTripTypes_PK")
                .IsClustered(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
