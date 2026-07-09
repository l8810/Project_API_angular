using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Services;
using StoreApi.DTOs;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _services;
        public OrderController(IOrderService services)
        {
            _services = services;
        }
        // ביצוע רכישה של עגלת קניות
        [Authorize]
        [HttpPost("buy")]
        public async Task<IActionResult> BuyCart([FromQuery] int userId)
        {
            var result = await _services.BuyCartAsync(userId);
            if (result == true)
            {
                return Ok("Cart purchased successfully.");
            }
            return BadRequest("Failed to purchase cart.");
        }
        // קבלת כל ההזמנות של משתמש
        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _services.GetOrdersByUserIdAsync(userId);
            // אין הזמנות זה מצב תקין ולא שגיאה — מחזירים מערך ריק (200)
            return Ok(orders ?? new List<OrderDto>());
        }
        // קבלת כל ההזמנות במערכת
        [Authorize(Roles = "2")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _services.GetAllOrdersAsync();
            // אין הזמנות זה מצב תקין ולא שגיאה — מחזירים מערך ריק (200)
            return Ok(orders ?? new List<OrderDto>());
        }
    }

}
