using StoreApi.Models;

namespace StoreApi.Repositories
{
    public interface IGiftRepository
    {
        Task<Gift> CreateGiftAsync(Gift gift);
        Task<bool> DeleteGiftAsync(int id);
        Task<List<Gift>> GetAllGiftsAsync();
        Task<Donor?> GetDonorByGiftIdAsync(int giftId);
        Task<Gift?> GetGiftByIdAsync(int id);
        Task<List<Gift>> GetGiftsByCategoryIdAsync(int categoryId);
        Task<List<Gift>> GetGiftsByDonorNameAsync(string donorName);
        Task<List<Gift>> GetGiftsByPurchaseCountAsync(int purchaseCount);
        Task<List<Gift>> SearchGiftsByNameAsync(string name);
        Task<Gift?> UpdateGiftAsync(int id, Gift gift);
        Task<User?> LotteryForGiftAsync(int giftId);
    }
}