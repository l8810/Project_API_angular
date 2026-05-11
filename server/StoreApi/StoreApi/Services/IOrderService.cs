using StoreApi.DTOs;
using StoreApi.Models;

namespace StoreApi.Services
{
    public interface IOrderService
    {
        Task<bool?> BuyCartAsync(int userId);
        Task<List<OrderDto>?> GetAllOrdersAsync();
        Task<List<OrderDto>?> GetOrdersByUserIdAsync(int userId);
    }
}