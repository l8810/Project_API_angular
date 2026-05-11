using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTOs;

namespace StoreApi.Repositories
{
    /// <summary>
    /// REPOSITORY LAYER - Data access for email notifications
    /// This layer retrieves winner information from the database
    /// </summary>
    public class EmailRepository : IEmailRepository
    {
        private readonly StoreContextDB _context;

        public EmailRepository(StoreContextDB context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the winner details needed to compose the congratulation email
        /// </summary>
        /// <param name="giftId">The ID of the gift that was won</param>
        /// <returns>WinnerDetailsDto containing winner email, name, and gift name</returns>
        public async Task<WinnerDetailsDto> GetWinnerDetailsAsync(int giftId)
        {
            return await _context.Gifts
                .Include(g => g.Winner)
                .Where(g => g.Id == giftId)
                .Select(g => new WinnerDetailsDto
                {
                    Email = g.Winner.Email,
                    WinnerName = g.Winner.Name,
                    GiftName = g.Name
                })
                .FirstOrDefaultAsync();
        }
    }
}
