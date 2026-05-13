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
        private readonly IEmailService _emailService;
        private readonly ILogger<GiftService> _logger;

        public GiftService(IGiftRepository repository, IEmailService emailService, ILogger<GiftService> logger)
        {
            _repository = repository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<List<GiftDtoWithCategoryAndDonor>> GetAllGiftsAsync()
        {
            var gifts = await _repository.GetAllGiftsAsync();
            _logger.LogInformation("Retrieved {Count} gifts", gifts.Count);
            return gifts.Select(g => MapToDetailedDto(g)).ToList();
        }

        public async Task<GiftDto?> GetGiftByIdAsync(int id)
        {
            var gift = await _repository.GetGiftByIdAsync(id);
            if (gift == null)
                _logger.LogWarning("Gift with ID {GiftId} not found", id);
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
            _logger.LogInformation("Created gift with ID {GiftId}, Name '{GiftName}'", created.Id, created.Name);
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
            if (updated == null)
            {
                _logger.LogWarning("Update failed — gift with ID {GiftId} not found", id);
                return null;
            }
            _logger.LogInformation("Updated gift with ID {GiftId}", id);
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

        public async Task<bool> DeleteGiftAsync(int id)
        {
            var result = await _repository.DeleteGiftAsync(id);
            if (result)
                _logger.LogInformation("Deleted gift with ID {GiftId}", id);
            else
                _logger.LogWarning("Delete failed — gift with ID {GiftId} not found", id);
            return result;
        }

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

        public async Task<UserWinerDTO?> LotteryForGiftAsync(int id)
        {
            _logger.LogInformation("Starting lottery for gift ID {GiftId}", id);
            var win = await _repository.LotteryForGiftAsync(id);

            if (win == null)
            {
                _logger.LogWarning("Lottery for gift ID {GiftId} had no participants", id);
                return null;
            }

            _logger.LogInformation("Lottery winner for gift ID {GiftId}: user ID {UserId} ({UserEmail})", id, win.Id, win.Email);
            await _emailService.SendWinnerNotificationAsync(id);

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