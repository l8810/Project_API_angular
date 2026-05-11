using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public class UserDto
    {
    }
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(50, ErrorMessage = "The name can't be longer than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is required.")]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        public string Password { get; set; }
    }

    public class UserUpdateDTO
    {
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(50, ErrorMessage = "The name can't be longer than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string Email { get; set; }
    }

    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string role { get; set; }
    }
    public class UserWinerDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(50, ErrorMessage = "The name can't be longer than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string Email { get; set; }
    }
}
