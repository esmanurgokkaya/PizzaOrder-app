using Microsoft.EntityFrameworkCore;
using PizzaOrderApp.Models;

namespace PizzaOrderApp.Data
{
    public class PizzaStoreContext : DbContext
    {
        public PizzaStoreContext(DbContextOptions<PizzaStoreContext> options) : base(options)
        {
        }

        // DbSet tanımlamaları - her model için
        public DbSet<Pizza> Pizzas { get; set; } = null!;
        public DbSet<PizzaSize> PizzaSizes { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<CustomerInfo> CustomerInfos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Pizza tablosu konfigürasyonu
            modelBuilder.Entity<Pizza>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasMaxLength(50);
                entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
                entity.Property(p => p.BasePrice).HasPrecision(10, 2).IsRequired();
                entity.Property(p => p.ImageUrl).HasMaxLength(500);
                entity.Property(p => p.IngredientsJson).HasDefaultValue("[]");
                
                // Pizza -> PizzaSize ilişkisi (1:N)
                entity.HasMany(p => p.Sizes)
                      .WithOne(s => s.Pizza)
                      .HasForeignKey(s => s.PizzaId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                // Pizza -> Order ilişkisi (1:N)
                entity.HasMany(p => p.Orders)
                      .WithOne(o => o.SelectedPizza)
                      .HasForeignKey(o => o.SelectedPizzaId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // PizzaSize tablosu konfigürasyonu
            modelBuilder.Entity<PizzaSize>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).HasMaxLength(50);
                entity.Property(s => s.Name).HasMaxLength(50).IsRequired();
                entity.Property(s => s.Multiplier).IsRequired();
                entity.Property(s => s.PizzaId).HasMaxLength(50).IsRequired();
                
                // PizzaSize -> Order ilişkisi (1:N)
                entity.HasMany(s => s.Orders)
                      .WithOne(o => o.SelectedSize)
                      .HasForeignKey(o => o.SelectedSizeId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Order tablosu konfigürasyonu
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.OrderNumber).HasMaxLength(50).IsRequired();
                entity.Property(o => o.TotalPrice).HasPrecision(10, 2).IsRequired();
                entity.Property(o => o.SelectedToppingsJson).HasDefaultValue("[]");
                entity.Property(o => o.OrderDate).IsRequired();
                
                entity.HasIndex(o => o.OrderNumber).IsUnique();
                
                // Order -> CustomerInfo ilişkisi (N:1)
                entity.HasOne(o => o.CustomerInfo)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(o => o.CustomerInfoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CustomerInfo tablosu konfigürasyonu
            modelBuilder.Entity<CustomerInfo>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).HasMaxLength(50).IsRequired();
                entity.Property(c => c.Email).HasMaxLength(100).IsRequired();
                entity.Property(c => c.Address).HasMaxLength(250).IsRequired();
                entity.Property(c => c.CreatedDate).IsRequired();
                
                entity.HasIndex(c => c.Email).IsUnique();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=pizzastore.db");
            }
        }
    }
}