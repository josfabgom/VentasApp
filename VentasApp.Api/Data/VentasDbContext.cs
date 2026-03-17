using Microsoft.EntityFrameworkCore;
using VentasApp.Api.Models;

namespace VentasApp.Api.Data
{
    public class VentasDbContext : DbContext
    {
        public VentasDbContext(DbContextOptions<VentasDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Equipo> Equipos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Semilla de base de datos
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "123", Role = "Admin" },
                new User { Id = 2, Username = "vendedor", Password = "123", Role = "Vendedor" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop Dell", Description = "Laptop para desarrollo", Price = 1200.50m, Stock = 10, Barcode = "1234567890", ImageUrl = "dotnet_bot.png" },
                new Product { Id = 2, Name = "Mouse Inalámbrico", Description = "Mouse ergonómico", Price = 25.00m, Stock = 50, Barcode = "0987654321", ImageUrl = "dotnet_bot.png" },
                new Product { Id = 3, Name = "Teclado Mecánico", Description = "Teclado RGB", Price = 75.00m, Stock = 20, Barcode = "1122334455", ImageUrl = "dotnet_bot.png" },
                new Product { Id = 4, Name = "Monitor LG 24'", Description = "Monitor IPS 1080p", Price = 150.00m, Stock = 15, Barcode = "5544332211", ImageUrl = "dotnet_bot.png" }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "Juan Pérez", Email = "juan@example.com", Phone = "555-0101", Address = "Calle Falsa 123" },
                new Customer { Id = 2, Name = "María Gómez", Email = "maria@example.com", Phone = "555-0202", Address = "Av. Siempre Viva 456" }
            );

            modelBuilder.Entity<Vendedor>().HasData(
                new Vendedor { Id = 1, Nombre = "Vendedor Test 1", Email = "vendedor1@test.com", Telefono = "111-222" },
                new Vendedor { Id = 2, Nombre = "Vendedor Test 2", Email = "vendedor2@test.com", Telefono = "333-444" }
            );

            modelBuilder.Entity<Equipo>().HasData(
                new Equipo { Id = 1, Nombre = "Samsung S24", Tipo = "Celular", NumeroSerie = "SN12345", VendedorId = 1 },
                new Equipo { Id = 2, Nombre = "Dell Latitude", Tipo = "PC", NumeroSerie = "SN67890", VendedorId = 2 }
            );
        }
    }
}
