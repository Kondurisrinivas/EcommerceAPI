using EcommerceAPI.Data;
using EcommerceAPI.DTO;
using EcommerceAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("Api/[Controller]")]
    public class ProductController : Controller
    {
        private readonly ECommerceDbContext _context;

        public ProductController(ECommerceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        //Get All Products
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return Ok(product);
        }

        [HttpGet("orders/{id}")]
        //Get Oders with ID
        public async Task<ActionResult<Product>> GetProductOrders(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound($"Product with {id} Not Found.");
            }

            var orders = await _context.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.Customer)
                .Where(oi => oi.ProductId == id)
                .Select(oi => new
                {
                    OrderId = oi.Order.Id,
                    OrderDate = oi.Order.OrderDate,
                    CustomerName = oi.Order.Customer.Name,
                    CustomerEmail = oi.Order.Customer.Email,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    OrderStatus = oi.Order.OrderStatus,
                    TotalAmount = oi.Order.OrderAmount
                })
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(new
            {
                ProductId = id,
                ProductName = product.Name,
                TotalOrders = orders.Count,
                Orders = orders
            });
        }
        //Get Customers who bought the Product with given Product ID
        [HttpGet("customers/{id}")]
        public async Task<ActionResult> GetProductCustomers(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("No Customers Found.");
            }

            var customers = await _context.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.Customer)
                .Where(oi => oi.ProductId == id)
                .Select(oi => oi.Order.Customer)
                .Distinct()
                .Select(c => new
                {
                    CustomerID = c.Id,
                    Customername = c.Name,
                    EmailAddress = c.Email,
                    TotalPurchases = _context.OrderItems
                        .Where(oi => oi.ProductId == id && oi.Order.CustomerId == c.Id)
                        .Sum(oi => oi.Quantity)
                })
                .ToListAsync();

            return Ok(new
            {
                ProductID = id,
                productName = product.Name,
                TotalCustomers = customers.Count,
                Customers = customers
            });
        }
        // Sales Stats of the Product with the given Product ID
        [HttpGet("Sales-Stats/{id}")]
        public async Task<ActionResult> GetProductSalesStats(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var orderItems = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.ProductId == id)
                .ToListAsync();

            var totalQuantitySold = orderItems.Sum(oi => oi.Quantity);
            var totalRevenue = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
            var averageOrderQuantity = orderItems.Any() ? orderItems.Average(oi => oi.Quantity) : 0;

            return Ok(new
            {
                ProductId = id,
                ProductName = product.Name,
                CurrentStock = product.Stock,
                TotalQuantitySold = totalQuantitySold,
                TotalRevenue = totalRevenue,
                AverageOrderQuantity = Math.Round(averageOrderQuantity, 2),
                NumberOfOrders = orderItems.Count
            });
        }

        // get product by Catagory
        // GET: Api/Products/category/Electronics
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
        {
            var products = await _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound($"No products found in category: {category}");
            }

            return Ok(products);
        }


        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProduct([FromQuery] String name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Please Enter text to search.");
            }


            var products = await _context.Products
                .Where(p => p.Name.ToLower() == name.ToLower())
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound($"No products found matching: {name}");
            }

            return Ok(products);
        }


        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductCreateDTO productcrtdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Name = productcrtdto.Name,
                Description = productcrtdto.Description,
                Category = productcrtdto.Category,
                Price = productcrtdto.Price,
                Stock = productcrtdto.Stock
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { product.Id }, product);
        }

        // PUT: Api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductCreateDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Stock = productDto.Stock;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok(product);
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }


        // DELETE: Api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            // Check if product is part of any orders
            var hasOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
            if (hasOrders)
            {
                return BadRequest("Cannot delete product that is part of existing orders. Consider marking it as out of stock instead.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }

        // GET: Api/Products/popular?limit=5
        [HttpGet("popular")]
        public async Task<ActionResult> GetPopularProducts([FromQuery] int limit = 5)
        {
            var popularProducts = await _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(limit)
                .ToListAsync();

            var productDetails = new List<object>();
            foreach (var item in popularProducts)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    productDetails.Add(new
                    {
                        Product = product,
                        TotalQuantitySold = item.TotalQuantitySold,
                        TotalOrders = item.TotalOrders,
                        TotalRevenue = item.TotalRevenue
                    });
                }
            }
            return Ok(productDetails);
        }
    }
}
