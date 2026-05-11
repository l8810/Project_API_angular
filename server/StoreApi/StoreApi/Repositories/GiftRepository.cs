using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models;

namespace StoreApi.Repositories
{
    public class GiftRepository : IGiftRepository
    {
        private readonly StoreContextDB _context;
        public GiftRepository(StoreContextDB context) => _context = context;

        public async Task<List<Gift>> GetAllGiftsAsync()
        {
            return await _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Include(g => g.Winner) 
                .ToListAsync();
        }

        public async Task<Gift?> GetGiftByIdAsync(int id)
        {
            return await _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Include(g => g.Winner)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Gift> CreateGiftAsync(Gift gift)
        {
            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();
            return gift;
        }

        public async Task<Gift?> UpdateGiftAsync(int id, Gift gift)
        {
            var existingGift = await _context.Gifts.FindAsync(id);
            if (existingGift == null) return null;

            existingGift.Name = gift.Name;
            existingGift.Description = gift.Description;
            existingGift.DonorId = gift.DonorId;
            existingGift.Price = gift.Price;
            existingGift.CategoryId = gift.CategoryId;
            existingGift.Picture = gift.Picture;
            existingGift.WinnerId = gift.WinnerId;

            await _context.SaveChangesAsync();
            return existingGift;
        }

        public async Task<bool> DeleteGiftAsync(int id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null) return false;

            var hasPurchases = await _context.ProductPurchaseds.AnyAsync(p => p.GiftId == id);
            if (hasPurchases) return false;

            _context.Gifts.Remove(gift);
            var cartItems = _context.ProductCarts.Where(c => c.GiftId == id);
            _context.ProductCarts.RemoveRange(cartItems);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Gift>> GetGiftsByDonorNameAsync(string donorName)
        {
            return await _context.Gifts
                .Include(g => g.Donor)
                .Include(g => g.Winner)
                .Where(g => g.Donor.Name == donorName)
                .ToListAsync();
        }

        public async Task<List<Gift>> GetGiftsByCategoryIdAsync(int categoryId)
        {
            return await _context.Gifts
                .Include(g => g.Winner)
                .Where(g => g.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<Gift>> SearchGiftsByNameAsync(string name)
        {
            return await _context.Gifts
                .Include(g => g.Winner)
                .Where(g => EF.Functions.Like(g.Name, $"%{name}%"))
                .ToListAsync();
        }

        public async Task<Donor?> GetDonorByGiftIdAsync(int giftId)
        {
            var gift = await _context.Gifts
                .Include(g => g.Donor)
                .FirstOrDefaultAsync(g => g.Id == giftId);
            return gift?.Donor;
        }

        public async Task<List<Gift>> GetGiftsByPurchaseCountAsync(int purchaseCount)
        {
            return await _context.Gifts
                .Include(g => g.Winner)
                .Where(g => g.Purchaseds.Count == purchaseCount)
                .ToListAsync();
        }

        public async Task<User?> LotteryForGiftAsync(int giftId)
        {
            var gift = await _context.Gifts
                .Include(g => g.Purchaseds)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(g => g.Id == giftId);

            if (gift == null || gift.Purchaseds.Count == 0) return null;

            var ticketPool = gift.Purchaseds
                .SelectMany(p => Enumerable.Repeat(p.User, p.Count))
                .ToList();

            var winner = ticketPool[new Random().Next(ticketPool.Count)];
            gift.WinnerId = winner.Id;

            await _context.SaveChangesAsync();
            return winner;
        }
    }
}