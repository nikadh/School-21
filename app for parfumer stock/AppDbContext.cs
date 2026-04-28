using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace PerfumeWarehouseWPF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=AppDbContext")
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<ProductNote> ProductNotes { get; set; }
        public DbSet<SamplerBox> SamplerBoxes { get; set; }
        public DbSet<SamplerBoxItem> SamplerBoxItems { get; set; }

        // Новые DbSet для заказов
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // --- Employees & Roles ---
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Employee>().HasIndex(e => e.LoginID).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(e => e.PassportNumber).IsUnique();
            modelBuilder.Entity<Employee>()
                .HasRequired(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleID);
            // SamplerBoxes
            modelBuilder.Entity<SamplerBox>().ToTable("SamplerBoxes");
            modelBuilder.Entity<SamplerBox>().HasIndex(b => b.BoxName).IsUnique();

            // SamplerBoxItems
            modelBuilder.Entity<SamplerBoxItem>().ToTable("SamplerBoxItems");
            modelBuilder.Entity<SamplerBoxItem>()
                .HasRequired(i => i.Box)
                .WithMany(b => b.Items)
                .HasForeignKey(i => i.BoxID);
            modelBuilder.Entity<SamplerBoxItem>()
                .HasRequired(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductID);

            // --- Brands ---
            modelBuilder.Entity<Brand>().ToTable("Brands");
            modelBuilder.Entity<Brand>().HasIndex(b => b.BrandName).IsUnique();

            // --- Products ---
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Product>().HasIndex(p => p.ProductName).IsUnique();
            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandID);

            // --- ProductVariants ---
            modelBuilder.Entity<ProductVariant>().ToTable("ProductVariants");
            modelBuilder.Entity<ProductVariant>()
                .HasRequired(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductID);
            modelBuilder.Entity<ProductVariant>()
                .HasIndex(v => new { v.ProductID, v.VolumeML })
                .IsUnique();

            // --- ProductImages ---
            modelBuilder.Entity<ProductImage>().ToTable("ProductImages");
            modelBuilder.Entity<ProductImage>()
                .HasRequired(img => img.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(img => img.ProductID);

            // --- Notes & ProductNotes ---
            modelBuilder.Entity<Note>().ToTable("Notes");
            modelBuilder.Entity<Note>().HasIndex(n => n.NoteName).IsUnique();
            modelBuilder.Entity<ProductNote>().ToTable("ProductNotes");
            modelBuilder.Entity<ProductNote>()
                .HasKey(pn => new { pn.ProductID, pn.NoteID });
            modelBuilder.Entity<ProductNote>()
                .HasRequired(pn => pn.Product)
                .WithMany(p => p.ProductNotes)
                .HasForeignKey(pn => pn.ProductID);
            modelBuilder.Entity<ProductNote>()
                .HasRequired(pn => pn.Note)
                .WithMany(n => n.ProductNotes)
                .HasForeignKey(pn => pn.NoteID);

            // --- Customers ---
            modelBuilder.Entity<Customer>().ToTable("Customers");

            // --- OrderStatuses ---
            modelBuilder.Entity<OrderStatus>().ToTable("OrderStatuses");
            modelBuilder.Entity<OrderStatus>().HasIndex(os => os.StatusName).IsUnique();

            // --- Orders ---
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID);
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Status)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusID);
            modelBuilder.Entity<Order>()
                .HasOptional(o => o.Employee)
                .WithMany()  // если у Employee нет коллекции Orders
                .HasForeignKey(o => o.EmployeeID);
            modelBuilder.Entity<Order>()
                .Property(o => o.FinalAmount)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            // --- OrderItems ---
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderID);
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Variant)
                .WithMany()
                .HasForeignKey(oi => oi.VariantID);
        }
    }
}