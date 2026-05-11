// Controllers/FileUploadController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [Authorize(Roles = "2")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // בדוק שזה תמונה
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Only images are allowed.");

            // צור שם קובץ ייחודי
            var fileName = $"{Guid.NewGuid()}{extension}";

            // צור תיקייה אם לא קיימת
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "gifts");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            // שמור את הקובץ
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // החזר את ה-URL של התמונה
            var imageUrl = $"/uploads/gifts/{fileName}";
            return Ok(new { url = imageUrl, fileName = fileName });
        }

        [HttpGet("image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "gifts", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var image = System.IO.File.OpenRead(filePath);
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            var mimeType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return File(image, mimeType);
        }
    }
}