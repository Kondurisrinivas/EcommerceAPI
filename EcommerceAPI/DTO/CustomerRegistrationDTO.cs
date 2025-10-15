using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.DTO
{
        // Data Transfer Object for customer registration.
        public class CustomerRegistrationDTO
        {
            [Required(ErrorMessage = "Customer name is required")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid Email")]
            public string Email { get; set; }
            [Required(ErrorMessage = "Password is required")]
            [StringLength(100)]
            public string Password { get; set; }
        }
}
