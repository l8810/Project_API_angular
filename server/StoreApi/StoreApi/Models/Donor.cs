using System.ComponentModel.DataAnnotations;

namespace StoreApi.Models
{
    public class Donor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The name is required.")]
        [StringLength(100, ErrorMessage = "The name can't be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "The address is required.")]
        [StringLength(200, ErrorMessage = "The address can't be longer than 200 characters.")]
        public string Address { get; set; }
        public List<Gift> Gifts { get; set; }= new List<Gift>();
    }
}
