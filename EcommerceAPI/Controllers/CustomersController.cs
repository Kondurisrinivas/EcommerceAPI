using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    public class CustomersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
