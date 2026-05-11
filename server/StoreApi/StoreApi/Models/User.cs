using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreApi.Models
{
    public enum Role
    {
        Manager,
        User
    }

    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(50, ErrorMessage = "The name can't be longer than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is required.")]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The role is required.")]
        public Role Role { get; set; } = Role.User;
        public List<Order> Orders { get; set; }= new List<Order>();
       public List<ProductCart> Cart { get; set; }=new List<ProductCart>();
    }
}
