using StoreApi.DTOs;

namespace StoreApi.Repositories
{
    public interface IEmailRepository
    {
        Task<WinnerDetailsDto> GetWinnerDetailsAsync(int giftId);
    }
}