using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class BuildingMaterialsShopContext : DbContext
    {
        public BuildingMaterialsShopContext()
        {
        }

        public BuildingMaterialsShopContext(DbContextOptions<BuildingMaterialsShopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Check> Checks { get; set; }
        public virtual DbSet<CheckDetail> CheckDetails { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Import> Imports { get; set; }
        public virtual DbSet<ImportDetail> ImportDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RefreshTokenCustomer> RefreshTokenCustomers { get; set; }
        public virtual DbSet<RefreshTokenEmployee> RefreshTokenEmployees { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Supply> Supplies { get; set; }
        public virtual DbSet<WareHouse> WareHouses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:BuildingMaterialShopDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasMaxLength(20);

                entity.Property(e => e.CategoryName).HasMaxLength(100);
            });

            modelBuilder.Entity<Check>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Checks)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_ChecksEmployee");
            });

            modelBuilder.Entity<CheckDetail>(entity =>
            {
                entity.HasKey(e => new { e.CheckId, e.ProductId })
                    .HasName("PK__CheckDet__4DC19B0A97E21D68");

                entity.Property(e => e.ProductId).HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(2000);

                entity.HasOne(d => d.Check)
                    .WithMany(p => p.CheckDetails)
                    .HasForeignKey(d => d.CheckId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckDetailsCheck");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CheckDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckDetailsProduct");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.BirthDay).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.IsBlocked).HasColumnName("isBlocked");

                entity.Property(e => e.PassWord).HasMaxLength(200);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.BirthDay).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.IsBlocked).HasColumnName("isBlocked");

                entity.Property(e => e.PassWord).HasMaxLength(200);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.RoleId).HasMaxLength(20);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_EmployeesRole");
            });

            modelBuilder.Entity<Import>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.SupplierId).HasMaxLength(20);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Imports)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_ImportsEmployee");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Imports)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_ImportsSupplier");
            });

            modelBuilder.Entity<ImportDetail>(entity =>
            {
                entity.HasKey(e => new { e.ImportId, e.ProductId })
                    .HasName("PK__ImportDe__4DD7AB860B18EF52");

                entity.Property(e => e.ProductId).HasMaxLength(20);

                entity.Property(e => e.Unit).HasMaxLength(100);

                entity.HasOne(d => d.Import)
                    .WithMany(p => p.ImportDetails)
                    .HasForeignKey(d => d.ImportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImportDetailsImport");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ImportDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImportDetailsProduct");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_OrdersCustomer");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId })
                    .HasName("PK__OrderDet__08D097A3DC84C355");

                entity.Property(e => e.ProductId).HasMaxLength(20);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetailsOrder");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetailsProduct");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasKey(e => new { e.StatusId, e.OrderId })
                    .HasName("PK__OrderSta__24D725DF2F6B3947");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderStatus)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderStatus_OrderId");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.OrderStatus)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderStatus_Status");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).HasMaxLength(20);

                entity.Property(e => e.CategoryId).HasMaxLength(20);

                entity.Property(e => e.Images).HasColumnType("text");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ProductName).HasMaxLength(100);

                entity.Property(e => e.Unit).HasMaxLength(100);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_ProductsCategory");
            });

            modelBuilder.Entity<RefreshTokenCustomer>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.ToTable("RefreshTokenCustomer");

                entity.Property(e => e.TokenId).HasColumnName("tokenId");

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expiryDate");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("token");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.RefreshTokenCustomers)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__RefreshTo__custo__3F466844");
            });

            modelBuilder.Entity<RefreshTokenEmployee>(entity =>
            {
                entity.HasKey(e => e.TokenId)
                    .HasName("PK__RefreshT__AC16DB47559C5F0A");

                entity.ToTable("RefreshTokenEmployee");

                entity.Property(e => e.TokenId).HasColumnName("tokenId");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expiryDate");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("token");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.RefreshTokenEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK__RefreshTo__emplo__403A8C7D");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasMaxLength(20);

                entity.Property(e => e.RoleName).HasMaxLength(100);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.StatusName).HasMaxLength(200);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.SupplierId).HasMaxLength(20);

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.SupplierName).HasMaxLength(100);
            });

            modelBuilder.Entity<Supply>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.SupplierId })
                    .HasName("PK__Supplies__E0B2A0A6C5BA613D");

                entity.Property(e => e.ProductId).HasMaxLength(20);

                entity.Property(e => e.SupplierId).HasMaxLength(20);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Supplies)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SuppliesProduct");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Supplies)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SuppliesSupplier");
            });

            modelBuilder.Entity<WareHouse>(entity =>
            {
                entity.ToTable("WareHouse");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasMaxLength(20);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.WareHouses)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_WareHouseProduct");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
