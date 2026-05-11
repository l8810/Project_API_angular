using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.DTOs;
using StoreApi.Services;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _services;
        public GiftController(IGiftService services)
        {
            _services = services;
        }
        //קבלת כל המתנות
        [HttpGet]
        public async Task<IActionResult> GetAllGifts()
        {
            var gifts = await _services.GetAllGiftsAsync();
            if (gifts == null || !gifts.Any())
            {
                return NotFound("No gifts found.");
            }
            return Ok(gifts);
        }
        //קבלת מתנה לפי מזהה
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftById(int id)
        {
            var gift = await _services.GetGiftByIdAsync(id);
            if (gift == null)
            {
                return NotFound($"Gift with ID {id} not found.");
            }
            return Ok(gift);
        }
        //יצירת מתנה
        [Authorize(Roles = "2")]
        [HttpPost]
        public async Task<IActionResult> CreateGift([FromBody] CreateGiftDto createGiftDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdGift = await _services.CreateGiftAsync(createGiftDto);
            return CreatedAtAction(nameof(GetGiftById), new { id = createdGift.Id }, createdGift);
        }
        //עדכון מתנה
        [Authorize(Roles = "2")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGift(int id, [FromBody] CreateGiftDto giftDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedGift = await _services.UpdateGiftAsync(id, giftDto);
            if (updatedGift == null)
            {
                return NotFound($"Gift with ID {id} not found.");
            }
            return Ok(updatedGift);
        }

        //מחיקת מתנה
        [Authorize(Roles = "2")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGift(int id)
        {
            var result = await _services.DeleteGiftAsync(id);
            if (!result)
            {
                return NotFound($"Gift with ID {id} not found.");
            }
            return NoContent();
        }
        //מיון לפי שם תורם
        [HttpGet("donor")]
        public async Task<IActionResult> GetGiftsByDonorName([FromQuery] string donorName)
        {
            var gifts = await _services.GetGiftsByDonorNameAsync(donorName);
            if (gifts == null || !gifts.Any())
            {
                return NotFound($"No gifts found for donor '{donorName}'.");
            }
            return Ok(gifts);
        }
        //מיון לפי קטגוריה
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetGiftsByCategoryId(int categoryId)
        {
            var gifts = await _services.GetGiftsByCategoryIdAsync(categoryId);
            if (gifts == null || !gifts.Any())
            {
                return NotFound($"No gifts found in category ID {categoryId}.");
            }
            return Ok(gifts);
        }
        //מיון לפי שם מתנה
        [HttpGet("search")]
        public async Task<IActionResult> SearchGiftsByName([FromQuery] string name)
        {
            var gifts = await _services.SearchGiftsByNameAsync(name);
            if (gifts == null || !gifts.Any())
            {
                return NotFound($"No gifts found with name containing '{name}'.");
            }
            return Ok(gifts);

        }
        //קבלת תורם לפי מזהה מתנה
        [HttpGet("{giftId}/donor")]
        public async Task<IActionResult> GetDonorByGiftId(int giftId)
        {
            var donor = await _services.GetDonorByGiftIdAsync(giftId);
            if (donor == null)
            {
                return NotFound($"No donor found for gift ID {giftId}.");
            }
            return Ok(donor);
        }
        //מיון לפי כמות רכישות
        [HttpGet("purchaseCount/{purchaseCount}")]
        public async Task<IActionResult> GetGiftsByPurchaseCount(int purchaseCount)
        {
            var gifts = await _services.GetGiftsByPurchaseCountAsync(purchaseCount);
            if (gifts == null || !gifts.Any())
            {
                return NotFound($"No gifts found with purchase count of {purchaseCount}.");
            }
            return Ok(gifts);

        }
        //הגרלת זוכה למתנה
        [Authorize(Roles = "2")]
        [HttpPost("{giftId}/lottery")]
        public async Task<IActionResult> LotteryForGift(int giftId)
        {
            var winner = await _services.LotteryForGiftAsync(giftId);
            if (winner == null)
            {
                return NotFound($"No participants found for gift ID {giftId}.");
            }
            return Ok(winner);
        }
    }
}
