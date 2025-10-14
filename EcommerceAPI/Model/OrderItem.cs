using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Model
{
    public class OrderItem
    {
            public int Id { get; set; }
            [Required]
            public int OrderId { get; set; }
            [JsonIgnore]
            public Order Order { get; set; }
            [Required]
            public int ProductId { get; set; }
            [JsonIgnore]
            public Product Product { get; set; }
            [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
            public int Quantity { get; set; }
            [Range(0.01, double.MaxValue, ErrorMessage = "Price cannot be a Negative Number")]
            [Column(TypeName = "decimal(18,2)")]
            public decimal UnitPrice { get; set; }
    }
}