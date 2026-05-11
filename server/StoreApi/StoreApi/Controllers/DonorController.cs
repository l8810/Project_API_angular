using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.DTOs;
using StoreApi.Services;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorController : ControllerBase
    {
        private readonly IDonorService _services;
        public DonorController(IDonorService services)
        {
            _services = services;
        }
        //קבלת כל התורמים
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllDonors()
        {
            var donors = await _services.GetAllDonorsAsync();
            if (donors == null || !donors.Any())
            {
                return NotFound("No donors found.");
            }
            return Ok(donors);
        }
        //קבלת תורם לפי מזהה
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDonorById(int id)
        {
            var donor = await _services.GetDonorByIdAsync(id);
            if (donor == null)
            {
                return NotFound($"Donor with ID {id} not found.");
            }
            return Ok(donor);
        }

        //יצירת תורם
        [Authorize(Roles = "2")]
        [HttpPost]
        public async Task<IActionResult> CreateDonor([FromBody] CreateDonorDto createDonorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdDonor = await _services.CreateDonorAsync(createDonorDto);
            return CreatedAtAction(nameof(GetDonorById), new { id = createdDonor.Id }, createdDonor);
        }
        //עדכון תורם
        [Authorize(Roles = "2")]
        //[HttpPut]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonor(int id, [FromBody] UpdateDonorDto updateDonorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedDonor = await _services.UpdateDonorAsync(id, updateDonorDto);
            if (updatedDonor == null)
            {
                return NotFound($"Donor with ID {id} not found.");
            }
            return Ok(updatedDonor);
        }
        //מחיקת תורם
        [Authorize(Roles = "2")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonor(int id)
        {
            var isDeleted = await _services.DeleteDonorAsync(id);
            if (!isDeleted)
            {
                return NotFound($"Donor with ID {id} not found.");
            }
            return NoContent();
        }
        //סינון לפי שם
        [Authorize]
        [HttpGet("searchByName")]
        public async Task<IActionResult> GetDonorsByName([FromQuery] string name)
        {
            var donors = await _services.GetDonorsByNameAsync(name);
            if (donors == null || !donors.Any())
            {
                return NotFound($"No donors found with name containing '{name}'.");
            }
            return Ok(donors);
        }
        //סינון לפי אימייל
        [Authorize]
        [HttpGet("searchByEmail")]
        public async Task<IActionResult> GetDonorsByEmail([FromQuery] string email)
        {
            var donors = await _services.GetDonorsByEmailAsync(email);
            if (donors == null || !donors.Any())
            {
                return NotFound($"No donors found with email containing '{email}'.");
            }
            return Ok(donors);
        }
        //סינון לפי מתנה
        [Authorize]
        [HttpGet("searchByGift")]
        public async Task<IActionResult> GetDonorsByGiftId([FromQuery] int giftId)
        {
            var donors = await _services.GetDonorsByGiftIdAsync(giftId);
            if (donors == null || !donors.Any())
            {
                return NotFound($"No donors found with gift containing '{giftId}'.");
            }
            return Ok(donors);
        }
    }
}
