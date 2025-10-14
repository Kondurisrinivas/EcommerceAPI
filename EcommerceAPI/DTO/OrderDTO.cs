using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.DTO
{
    public class OrderDTO
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public List<OrderItemDTO> Items { get; set; }
    }
}
