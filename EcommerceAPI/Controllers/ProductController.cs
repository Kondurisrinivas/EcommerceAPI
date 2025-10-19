using EcommerceAPI.Data;
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
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products =await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("Orders/{id}")]
        public async Task<ActionResult<Product>> GetProductByID(int id)
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

    }
}
