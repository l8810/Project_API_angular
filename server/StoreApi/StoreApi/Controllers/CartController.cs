using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.DTOs;
using StoreApi.Services;

namespace StoreApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _services;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService services, ILogger<CartController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CreateCartDto createCartDto)
        {
            _logger.LogInformation("POST /cart — user {UserId} adding gift {GiftId}", createCartDto.UserId, createCartDto.GiftId);
            var result = await _services.AddToCartAsync(createCartDto);
            if (result == true)
                return Ok("Item added to cart successfully.");

            _logger.LogWarning("POST /cart — failed to add gift {GiftId} for user {UserId}", createCartDto.GiftId, createCartDto.UserId);
            return BadRequest("Failed to add item to cart.");
        }

        [HttpGet("GetCartByUserId")]
        public async Task<IActionResult> GetCartByUserId([FromQuery] int userId)
        {
            var cartItems = await _services.GetCartByUserIdAsync(userId);
            return Ok(cartItems ?? new List<CartDto>());
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            _logger.LogInformation("DELETE /cart/{CartItemId}", cartItemId);
            var result = await _services.RemoveFromCartAsync(cartItemId);
            if (result == true)
                return Ok("Item removed from cart successfully.");

            _logger.LogWarning("DELETE /cart/{CartItemId} — not found", cartItemId);
            return NotFound("Cart item not found.");
        }

        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromQuery] int addCount)
        {
            _logger.LogInformation("PUT /cart/{CartItemId} addCount={AddCount}", cartItemId, addCount);
            var result = await _services.UpdateCartItemAsync(cartItemId, addCount);
            if (result == true)
                return Ok("Cart item updated successfully.");

            _logger.LogWarning("PUT /cart/{CartItemId} — not found", cartItemId);
            return NotFound("Cart item not found.");
        }
    }
}
