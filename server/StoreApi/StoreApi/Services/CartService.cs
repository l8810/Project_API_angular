using Microsoft.AspNetCore.Cors.Infrastructure;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Repositories;

namespace StoreApi.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;

        public CartService(ICartRepository cartRepository)
        {
            _repository = cartRepository;
        }

        // הוספת מוצר לעגלה
        public async Task<bool?> AddToCartAsync(CreateCartDto createCartDto)
        {
            var cart = new ProductCart
            {
                UserId = createCartDto.UserId,
                GiftId = createCartDto.GiftId,
                Count = createCartDto.Count
            };

            var IsAdd = await _repository.AddToCartAsync(cart);
            return IsAdd;
        }

        // קבלת עגלת קניות של משתמש
        public async Task<List<CartDto>> GetCartByUserIdAsync(int userId)
        {
            var cartItems = await _repository.GetCartByUserIdAsync(userId);

            return cartItems.Select(c => new CartDto
            {
                Id = c.Id,
                GiftId = c.GiftId,
                GiftName = c.Gift?.Name, 
                GiftImage = c.Gift?.Picture, 
                Price = c.Gift?.Price ?? 0, 
                UserId = c.UserId,
                Count = c.Count
            }).ToList();
        }

        // הסרת מוצר מהעגלה
        public async Task<bool?> RemoveFromCartAsync(int cartItemId)
        {
            return await _repository.RemoveFromCartAsync(cartItemId);
        }

        // עדכון כמות מוצר בעגלה
        public async Task<bool?> UpdateCartItemAsync(int cartItemId, int addCount)
        {
            return await _repository.UpdateCartItemAsync(cartItemId, addCount);
        }
    }
}
