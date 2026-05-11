using StoreApi.Models;

namespace StoreApi.Repositories
{
    public interface ICartRepository
    {
        Task<bool?> AddToCartAsync(ProductCart cart);
        Task<List<ProductCart>> GetCartByUserIdAsync(int userId);
        Task<bool?> RemoveFromCartAsync(int cartItemId);
        Task<bool> UpdateCartItemAsync(int cartItemId, int newCount);
    }
}