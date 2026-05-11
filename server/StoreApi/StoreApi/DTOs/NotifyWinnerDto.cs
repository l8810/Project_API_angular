using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public class NotifyWinnerDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string GiftName { get; set; }
    }
}