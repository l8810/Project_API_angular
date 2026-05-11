using StoreApi.DTOs;

namespace StoreApi.Services
{
    public interface IDonorService
    {
        Task<DonorDto> CreateDonorAsync(CreateDonorDto createDonorDto);
        Task<bool> DeleteDonorAsync(int id);
        Task<List<DonorDto>> GetAllDonorsAsync();
        Task<DonorDtoById?> GetDonorByIdAsync(int id);
        Task<List<DonorDto>> GetDonorsByEmailAsync(string email);
        Task<List<DonorDto>> GetDonorsByGiftIdAsync(int giftId);
        Task<List<DonorDto>> GetDonorsByNameAsync(string name);
        Task<DonorDtoById?> UpdateDonorAsync(int id, UpdateDonorDto updateDonorDto);
    }
}