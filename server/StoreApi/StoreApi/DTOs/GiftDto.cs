using StoreApi.Models;
using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public class GiftDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The gift name is required.")]
        [MaxLength(100, ErrorMessage = "The gift name can't be longer than 100 characters.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "The description can't be longer than 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The donor is required.")]
        public int DonorId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "The category is required.")]
        public int CategoryId { get; set; }
        public string Picture { get; set; }
        public int? WinnerId { get; set; }
        public string? WinnerName { get; set; }
    }
    public class CreateGiftDto
    {
        [Required(ErrorMessage = "The gift name is required.")]
        [MaxLength(100, ErrorMessage = "The gift name can't be longer than 100 characters.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "The description can't be longer than 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The donor is required.")]
        public int DonorId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "The category is required.")]
        public int CategoryId { get; set; }
        public string Picture { get; set; }
        public int? WinnerId { get; set; }
        public string? WinnerName { get; set; }
    }

    public class GiftDtoWithCategoryAndDonor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DonorId { get; set; }       
        public string? DonorName { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }   
        public string? CategoryName { get; set; }
        public string Picture { get; set; }
        public int? WinnerId { get; set; }
        public string? WinnerName { get; set; }
    }

}
