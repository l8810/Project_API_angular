using System.ComponentModel.DataAnnotations;

namespace StoreApi.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The category name is required.")]
        [StringLength(100, ErrorMessage = "The category name can't be longer than 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "The description can't be longer than 500 characters.")]
        public string Description { get; set; }

        public List<Gift> Gifts { get; set; } = new List<Gift>();
    }
}
