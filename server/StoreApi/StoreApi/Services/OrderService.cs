using System.Linq;
using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Repositories;

namespace StoreApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        public OrderService(IOrderRepository repository)
        {
            this._repository = repository;
        }

        // ביצוע רכישה של עגלת קניות
        public async Task<bool?> BuyCartAsync(int userId)
        {
            // Repository now returns the created Order (or null on failure)
            var order = await _repository.BuyCartAsync(userId);
            if (order == null)
                return false;

            // Prepare a simple summary for email
            var summaryLines = new List<string>();
            summaryLines.Add($"Order Id: {order.id}");
            summaryLines.Add($"Order Date: {order.orderDate}");
            summaryLines.Add("Items:");
            foreach (var pp in order.listOrder ?? new List<ProductPurchased>())
            {
                var name = pp.Gift?.Name ?? "Unknown";
                summaryLines.Add($"- {name} x{pp.Count} (price: {pp.Gift?.Price})");
            }
            var orderSummary = string.Join("\n", summaryLines);

            // Email sending removed per request; purchase completed regardless

            return true;
        }

        // קבלת כל ההזמנות של משתמש
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
        // קבלת כל ההזמנות של כל המשתמשים
        public async Task<List<OrderDto>?> GetAllOrdersAsync()
        {
            var orders = await _repository.GetAllOrdersAsync();
            if (orders == null) return null;

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
