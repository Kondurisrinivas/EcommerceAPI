using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Model
{
    public class Customer
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Customer name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100)]
        public string Password { get; set; }
        [JsonIgnore]
        public List<Order> Orders { get; set; }
    }
}
