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
        private readonly ILogger<CartService> _logger;

        public CartService(ICartRepository cartRepository, ILogger<CartService> logger)
        {
            _repository = cartRepository;
            _logger = logger;
        }

        public async Task<bool?> AddToCartAsync(CreateCartDto createCartDto)
        {
            _logger.LogInformation("User {UserId} adding gift {GiftId} (qty {Count}) to cart",
                createCartDto.UserId, createCartDto.GiftId, createCartDto.Count);

            var cart = new ProductCart
            {
                UserId = createCartDto.UserId,
                GiftId = createCartDto.GiftId,
                Count = createCartDto.Count
            };

            var result = await _repository.AddToCartAsync(cart);
            if (result != true)
                _logger.LogWarning("Failed to add gift {GiftId} to cart for user {UserId}", createCartDto.GiftId, createCartDto.UserId);
            return result;
        }

        public async Task<List<CartDto>> GetCartByUserIdAsync(int userId)
        {
            var cartItems = await _repository.GetCartByUserIdAsync(userId);
            _logger.LogInformation("Retrieved {Count} cart items for user {UserId}", cartItems.Count, userId);

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

        public async Task<bool?> RemoveFromCartAsync(int cartItemId)
        {
            var result = await _repository.RemoveFromCartAsync(cartItemId);
            if (result == true)
                _logger.LogInformation("Removed cart item {CartItemId}", cartItemId);
            else
                _logger.LogWarning("Remove failed — cart item {CartItemId} not found", cartItemId);
            return result;
        }

        public async Task<bool?> UpdateCartItemAsync(int cartItemId, int addCount)
        {
            var result = await _repository.UpdateCartItemAsync(cartItemId, addCount);
            if (result == true)
                _logger.LogInformation("Updated cart item {CartItemId} by {AddCount}", cartItemId, addCount);
            else
                _logger.LogWarning("Update failed — cart item {CartItemId} not found", cartItemId);
            return result;
        }
    }
}
