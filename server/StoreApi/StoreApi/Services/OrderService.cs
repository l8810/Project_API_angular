using System.Linq;
using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Repositories;

namespace StoreApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository repository, ILogger<OrderService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool?> BuyCartAsync(int userId)
        {
            _logger.LogInformation("User {UserId} initiated checkout", userId);
            var order = await _repository.BuyCartAsync(userId);
            if (order == null)
            {
                _logger.LogWarning("Checkout failed for user {UserId} — cart may be empty", userId);
                return false;
            }

            var itemCount = order.listOrder?.Count ?? 0;
            _logger.LogInformation("Order {OrderId} created for user {UserId} with {ItemCount} item(s)", order.id, userId, itemCount);
            return true;
        }

        public async Task<List<OrderDto>?> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _repository.GetOrdersByUserIdAsync(userId);
            if (orders == null)
                return null;
            return orders.Select(o => new OrderDto
            {
                id = o.id,
                UserId = o.UserId,
                orderDate = o.orderDate, 
                listOrder = o.listOrder?.Select(pp => new ProductPurchasedDto
                {
                    Id = pp.Id,
                    GiftId = pp.GiftId,
                    UserId = pp.UserId,
                    Count = pp.Count,
                    giftName = pp.Gift.Name,
                    price = pp.Gift.Price,
                    picture = pp.Gift.Picture
                }).ToList() ?? new List<ProductPurchasedDto>()
            }).ToList();
        }
        public async Task<List<OrderDto>?> GetAllOrdersAsync()
        {
            var orders = await _repository.GetAllOrdersAsync();
            if (orders == null) return null;
            _logger.LogInformation("Retrieved {Count} total orders", orders.Count);

            return orders.Select(o => new OrderDto
            {
                id = o.id,
                UserId = o.UserId,
                userName = o.user?.Name, 
                orderDate = o.orderDate,
                listOrder = o.listOrder?.Select(pp => new ProductPurchasedDto
                {
                    Id = pp.Id,
                    GiftId = pp.GiftId,
                    UserId = pp.UserId,
                    Count = pp.Count,
                    giftName = pp.Gift.Name,
                    price = pp.Gift.Price,
                    picture=pp.Gift.Picture
                }).ToList() ?? new List<ProductPurchasedDto>()
            }).ToList();
        }
    }
    }
