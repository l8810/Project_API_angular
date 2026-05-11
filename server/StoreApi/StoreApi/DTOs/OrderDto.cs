using StoreApi.Models;
using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public class OrderDto
    {
        public int id { get; set; }

        [Required(ErrorMessage = "The user is required.")]
        public int UserId { get; set; }

        public string userName { get; set; }

        [Required(ErrorMessage = "The order date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime orderDate { get; set; }

        [Required(ErrorMessage = "The order must contain at least one product.")]
        public ICollection<ProductPurchasedDto> listOrder { get; set; } = new List<ProductPurchasedDto>();
    }
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "The user is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "The order must contain at least one product.")]
        public ICollection<ProductPurchasedDto> listOrder { get; set; } = new List<ProductPurchasedDto>();
    }
}
