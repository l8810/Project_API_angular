using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Models;

namespace StoreApi.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly StoreContextDB _context;
        public CartRepository(StoreContextDB context)
        {
            _context = context;
        }
        //הוספת מוצר לעגלה

        public async Task<bool?> AddToCartAsync(ProductCart cart)
        {
            //var user = await _context.Users.FindAsync(cart.UserId);
            var user=await _context.Users.Include(x=>x.Cart).FirstOrDefaultAsync(x=>x.Id==cart.UserId);
            if (user == null)
            {
                return false;
            }
            var existingCartItem = user.Cart.FirstOrDefault(c => c.GiftId == cart.GiftId);
            if (existingCartItem != null)
            {
                existingCartItem.Count += cart.Count;
            }
            else
            {
                _context.ProductCarts.Add(cart);
            }
            await _context.SaveChangesAsync();
                return true;
        }

        //קבלת עגלת קניות של משתמש
        public async Task<List<ProductCart>> GetCartByUserIdAsync(int userId)
        {
            return await _context.ProductCarts
                .Include(c => c.Gift) 
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }


        //הסרת מוצר מהעגלה
        public async Task<bool?> RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.ProductCarts.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return false;
            }
            _context.ProductCarts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }
        //עדכון כמות מוצר בעגלה
        public async Task<bool> UpdateCartItemAsync(int cartItemId, int addCount)
        {
            var cartItem = await _context.ProductCarts.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return false;
            }
            //cartItem.Count = newCount;
            cartItem.Count += addCount;
            await _context.SaveChangesAsync();
            if (cartItem.Count <= 0)
            {
                _context.ProductCarts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
