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
        public CartController(ICartService services)
        {
            _services = services;
        }

        // הוספת מוצר לעגלה
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CreateCartDto createCartDto)
        {
            var result = await _services.AddToCartAsync(createCartDto);
            if (result == true)
            {
                return Ok("Item added to cart successfully.");
            }
            return BadRequest("Failed to add item to cart.");
        }
        // קבלת עגלת קניות של משתמש
        [HttpGet("GetCartByUserId")]
        public async Task<IActionResult> GetCartByUserId([FromQuery] int userId)
        {
            var cartItems = await _services.GetCartByUserIdAsync(userId);
            return Ok(cartItems ?? new List<CartDto>());
        }
        // הסרת מוצר מהעגלה
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var result = await _services.RemoveFromCartAsync(cartItemId);
            if (result == true)
            {
                return Ok("Item removed from cart successfully.");
            }
            return NotFound("Cart item not found.");
        }
        // עדכון כמות מוצר בעגלה
        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromQuery] int addCount)
        {
            var result = await _services.UpdateCartItemAsync(cartItemId, addCount);
            if (result == true)
            {
                return Ok("Cart item updated successfully.");
            }
            return NotFound("Cart item not found.");
        }

    }
}
