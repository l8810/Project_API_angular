using StoreApi.Models;

namespace StoreApi.Repositories
{
    public interface IDonorRepository
    {
        Task<Donor> CreateDonorAsync(Donor donor);
        Task<bool> DeleteDonorAsync(int id);
        Task<List<Donor>> GetAllDonorsAsync();
        Task<Donor?> GetDonorByIdAsync(int id);
        Task<List<Donor>> GetDonorsByEmailAsync(string email);
        Task<List<Donor>> GetDonorsByGiftIdAsync(int giftId);
        Task<List<Donor>> GetDonorsByNameAsync(string name);
        Task<Donor?> UpdateDonorAsync(int id, Donor donor);
    }
}