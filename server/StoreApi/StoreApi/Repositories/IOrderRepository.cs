using StoreApi.Models;

namespace StoreApi.Repositories
{
    public interface IOrderRepository
    {
        Task<StoreApi.Models.Order?> BuyCartAsync(int userId);
        Task<List<Order>?> GetAllOrdersAsync();
        Task<List<Order>?> GetOrdersByUserIdAsync(int userId);
    }
}