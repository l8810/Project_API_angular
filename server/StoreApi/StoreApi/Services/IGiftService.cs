using StoreApi.DTOs;

namespace StoreApi.Services
{
    public interface IGiftService
    {
        Task<GiftDtoWithCategoryAndDonor> CreateGiftAsync(CreateGiftDto createGiftDto);
        Task<bool> DeleteGiftAsync(int id);
        Task<List<GiftDtoWithCategoryAndDonor>> GetAllGiftsAsync();
        Task<DonorDto?> GetDonorByGiftIdAsync(int giftId);
        Task<GiftDto?> GetGiftByIdAsync(int id);
        Task<List<GiftDto>> GetGiftsByCategoryIdAsync(int categoryId);
        Task<List<GiftDto>> GetGiftsByDonorNameAsync(string donorName);
        Task<List<GiftDto>> GetGiftsByPurchaseCountAsync(int purchaseCount);
        Task<List<GiftDto>> SearchGiftsByNameAsync(string name);
        Task<CreateGiftDto?> UpdateGiftAsync(int id, CreateGiftDto giftDto);
        Task<UserWinerDTO?> LotteryForGiftAsync(int giftId);
    }
}