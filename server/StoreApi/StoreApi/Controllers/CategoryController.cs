using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.DTOs;
using StoreApi.Services;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _services;
        public CategoryController(ICategoryService services)
        {
            _services = services;
        }

        //קבלת כל הקטגוריות
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _services.GetAllCategoriesAsync();
            // אוסף ריק אינו שגיאה — מחזירים 200 עם מערך ריק
            return Ok(categories);
        }

        //קבלת קטגוריה לפי מזהה
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _services.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(category);
        }

        //יצירת קטגוריה
        [Authorize(Roles = "2")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdCategory = await _services.CreateCategoryAsync(createCategoryDto);
            //מה זה?
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }
        //עדכון קטגוריה
        [Authorize(Roles = "2")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedCategory = await _services.UpdateCategoryAsync(id, updateCategoryDto);
            if (updatedCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(updatedCategory);
        }
        //מחיקת קטגוריה
        [Authorize(Roles = "2")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeleted = await _services.DeleteCategoryAsync(id);
            if (!isDeleted)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
