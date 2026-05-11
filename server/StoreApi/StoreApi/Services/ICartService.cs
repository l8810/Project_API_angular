using StoreApi.DTOs;

namespace StoreApi.Services
{
    public interface ICartService
    {
        Task<bool?> AddToCartAsync(CreateCartDto createCartDto);
        Task<List<CartDto>> GetCartByUserIdAsync(int userId);
        Task<bool?> RemoveFromCartAsync(int cartItemId);
        Task<bool?> UpdateCartItemAsync(int cartItemId, int addCount);
    }
}