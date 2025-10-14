using EcommerceAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EcommerceAPI.Data
{
    public class ECommerceDbContext : DbContext
    {
       public ECommerceDbContext (DbContextOptions< ECommerceDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed data for Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice.johnson@example.com",
                    Password = "Password123" // Note: In real applications, passwords 
                },
                new Customer
                {
                    Id = 2,
                    Name = "Bob Smith",
                    Email = "bob.smith@example.com",
                    Password = "Password456"
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "A high-performance laptop.",
                    Category = "Electronics",
                    Price = 1200.00m,
                    Stock = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "Smartphone",
                    Description = "Latest model smartphone.",
                    Category = "Electronics",
                    Price = 800.00m,
                    Stock = 25
                },
                new Product
                {
                    Id = 3,
                    Name = "Headphones",
                    Description = "Noise-cancelling headphones.",
                    Category = "Accessories",
                    Price = 150.00m,
                    Stock = 50
                }
            );
            // Seed data for Orders
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    OrderDate = new DateTime(2024, 12, 1),
                    CustomerId = 1,
                    OrderStatus = "Shipped",
                    OrderAmount = 2000.00m
                },
                new Order
                {
                    Id = 2,
                    OrderDate = new DateTime(2025, 01, 25),
                    CustomerId = 2,
                    OrderStatus = "Processing",
                    OrderAmount = 950.00m
                }
            );
            // Seed data for OrderItems
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    UnitPrice = 1200.00m
                },
                new OrderItem
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 2,
                    Quantity = 1,
                    UnitPrice = 800.00m
                },
                new OrderItem
                {
                    Id = 3,
                    OrderId = 2,
                    ProductId = 3,
                    Quantity = 2,
                    UnitPrice = 150.00m
                },
                new OrderItem
                {
                    Id = 4,
                    OrderId = 2,
                    ProductId = 2,
                    Quantity = 1,
                    UnitPrice = 800.00m
                }
            );
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}