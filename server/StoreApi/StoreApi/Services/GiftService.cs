using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Repositories;

namespace StoreApi.Services
{
    /// <summary>
    /// SERVICE LAYER - Business logic for gift management
    /// This service coordinates between controllers and repositories
    /// </summary>
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _repository;
        private readonly IEmailService _emailService; // NEW: Email service injection

        public GiftService(IGiftRepository repository, IEmailService emailService) // NEW: Added email service parameter
        {
            _repository = repository;
            _emailService = emailService; // NEW: Store email service
        }

        public async Task<List<GiftDtoWithCategoryAndDonor>> GetAllGiftsAsync()
        {
            var gifts = await _repository.GetAllGiftsAsync();
            return gifts.Select(g => MapToDetailedDto(g)).ToList();
        }

        public async Task<GiftDto?> GetGiftByIdAsync(int id)
        {
            var gift = await _repository.GetGiftByIdAsync(id);
            return gift == null ? null : MapToDto(gift);
        }

        public async Task<GiftDtoWithCategoryAndDonor> CreateGiftAsync(CreateGiftDto dto)
        {
            var gift = new Gift
            {
                Name = dto.Name,
                Description = dto.Description,
                DonorId = dto.DonorId,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                Picture = dto.Picture
            };
            var created = await _repository.CreateGiftAsync(gift);
            var detailed = await _repository.GetGiftByIdAsync(created.Id);
            return MapToDetailedDto(detailed!);
        }

        public async Task<CreateGiftDto?> UpdateGiftAsync(int id, CreateGiftDto dto)
        {
            var gift = new Gift
            {
                Name = dto.Name,
                Description = dto.Description,
                DonorId = dto.DonorId,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                Picture = dto.Picture
            };
            var updated = await _repository.UpdateGiftAsync(id, gift);
            if (updated == null) return null;
            return new CreateGiftDto
            {
                Name = updated.Name,
                Description = updated.Description,
                DonorId = updated.DonorId,
                Price = updated.Price,
                CategoryId = updated.CategoryId,
                Picture = updated.Picture,
                WinnerId = updated.WinnerId,
                WinnerName = updated.Winner?.Name
            };
        }

        public async Task<bool> DeleteGiftAsync(int id) => await _repository.DeleteGiftAsync(id);

        public async Task<List<GiftDto>> GetGiftsByDonorNameAsync(string name) =>
            (await _repository.GetGiftsByDonorNameAsync(name)).Select(MapToDto).ToList();

        public async Task<List<GiftDto>> GetGiftsByCategoryIdAsync(int id) =>
            (await _repository.GetGiftsByCategoryIdAsync(id)).Select(MapToDto).ToList();

        public async Task<List<GiftDto>> SearchGiftsByNameAsync(string name) =>
            (await _repository.SearchGiftsByNameAsync(name)).Select(MapToDto).ToList();

        public async Task<DonorDto?> GetDonorByGiftIdAsync(int id)
        {
            var d = await _repository.GetDonorByGiftIdAsync(id);
            return d == null ? null : new DonorDto { Id = d.Id, Name = d.Name, Email = d.Email, Phone = d.Phone, Address = d.Address };
        }

        public async Task<List<GiftDto>> GetGiftsByPurchaseCountAsync(int count) =>
            (await _repository.GetGiftsByPurchaseCountAsync(count)).Select(MapToDto).ToList();

        /// <summary>
        /// Conducts the lottery and sends email to winner
        /// NEW: Automatically sends congratulation email when someone wins
        /// </summary>
        public async Task<UserWinerDTO?> LotteryForGiftAsync(int id)
        {
            // STEP 1: Conduct the lottery (Repository Layer)
            var win = await _repository.LotteryForGiftAsync(id);
            
            if (win == null)
                return null;

            // STEP 2: Send congratulation email to winner (Service Layer - Email Service)
            // This automatically sends the email with winner name and gift details
            try
            {
                await _emailService.SendWinnerNotificationAsync(id);
            }
            catch (Exception ex)
            {
                // Email sending failure doesn't prevent lottery success
                // Log this error but continue
                Console.WriteLine($"Warning: Email notification failed for winner: {ex.Message}");
            }

            // STEP 3: Return winner details to caller
            return new UserWinerDTO { Id = win.Id, Name = win.Name, Email = win.Email };
        }

        private GiftDto MapToDto(Gift g) => new GiftDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            DonorId = g.DonorId,
            Price = g.Price,
            CategoryId = g.CategoryId,
            Picture = g.Picture,
            WinnerId = g.WinnerId,
            WinnerName = g.Winner?.Name
        };

        private GiftDtoWithCategoryAndDonor MapToDetailedDto(Gift g) => new GiftDtoWithCategoryAndDonor
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            DonorId = g.DonorId,
            DonorName = g.Donor?.Name,
            Price = g.Price,
            CategoryId = g.CategoryId,
            CategoryName = g.Category?.Name,
            Picture = g.Picture,
            WinnerId = g.WinnerId,
            WinnerName = g.Winner?.Name,
        };
    }
}