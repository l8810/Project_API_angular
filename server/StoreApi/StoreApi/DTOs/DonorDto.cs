using StoreApi.Models;
using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public class DonorDto
    {
        // אם מורידים את הId יש לשנות גם את הService ואת הRepository
        // זה עושה בעיה בהחזרה של כל הCreateCategory שאין לו Id לבדוק מה המשמעות של ההחזרה בלי Id
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
    }
    public class DonorDtoById
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
        public List<Gift> Gifts { get; set; } = new List<Gift>();
    }
    public class CreateDonorDto
    {
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
    }
    public class UpdateDonorDto
    {
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
    }
}
