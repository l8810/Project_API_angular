using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models;

namespace StoreApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreContextDB _context;
        public OrderRepository(StoreContextDB context)
        {
            _context = context;
        }

        // ביצוע רכישה של עגלת קניות
        public async Task<Order?> BuyCartAsync(int userId)
        {
            var user = await _context.Users.Include(x => x.Cart).FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return null;
            }
            var cartItems = user.Cart.ToList();
            if (!cartItems.Any())
            {
                return null;
            }
            Order order = new Order
            {
                UserId = userId,
                orderDate = DateTime.Now,
                listOrder = new List<ProductPurchased>()
            };

            foreach (var item in cartItems)
            {
                var productPurchased = new ProductPurchased
                {
                    GiftId = item.GiftId,
                    UserId = userId,
                    Count = item.Count
                };
                order.listOrder.Add(productPurchased);
                _context.ProductCarts.Remove(item);
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Load the order with related data (gifts and user) for service usage
            await _context.Entry(order).Reference(o => o.user).LoadAsync();
            await _context.Entry(order).Collection(o => o.listOrder).Query().Include(pp => pp.Gift).LoadAsync();

            return order;
        }

        // קבלת כל ההזמנות של משתמש
        public async Task<List<Order>?> GetOrdersByUserIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.listOrder)
                        .ThenInclude(pp => pp.Gift) 
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return user.Orders.ToList();
        }

              
        public async Task<List<Order>?> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o=>o.user)
                .Include(o => o.listOrder)
                    .ThenInclude(pp => pp.Gift)
                .ToListAsync();
        }
    }
}
