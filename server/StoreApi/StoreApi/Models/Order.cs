using System.ComponentModel.DataAnnotations;

namespace StoreApi.Models
{
    public class Order
    {
        public int id { get; set; }

        [Required(ErrorMessage = "The user is required.")]
        public int UserId { get; set; }
        public User user { get; set; }

        [Required(ErrorMessage = "The order date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime orderDate { get; set; }

        [Required(ErrorMessage = "The order must contain at least one product.")]
        public List<ProductPurchased> listOrder { get; set; }= new List<ProductPurchased>();
    }
}
