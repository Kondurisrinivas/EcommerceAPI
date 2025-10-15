using EcommerceAPI.Data;
using EcommerceAPI.Model;
using EcommerceAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [Route("Api/[Controller]")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly ECommerceDbContext _context;
        
        public CustomersController(ECommerceDbContext context)
        {
            _context = context;
        }


        [HttpPost("register")]
        public async Task<ActionResult<Customer>> RegisterCustomer([FromBody] CustomerRegistrationDTO registrationDto)// Laso Used FromBody to get give the raw data input with jsoon
        {
            if(await _context.Customers.AnyAsync(c=> c.Email == registrationDto.Email))
            {
                return BadRequest("Email Already Exists.");
            }

            var customer = new Customer
            {
                Name = registrationDto.Name,
                Email = registrationDto.Email,
                Password= registrationDto.Password
            };

            _context.Add(customer);

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                return Ok(customer);
            }
            return NotFound();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromHeader(Name="X-Client-ID")] String clientId,[FromBody] CustomerLoginDTO loginDTO)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return BadRequest("Missing Client ID");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == loginDTO.Email && c.Password == loginDTO.Password);

            if (customer == null)
            {
                return Unauthorized("Invalid Email or Password. Please check.");
            }

            return Ok("Successfully Logged in");
            
        }

    }
}
