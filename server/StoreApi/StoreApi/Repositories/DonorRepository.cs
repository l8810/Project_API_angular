using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Models;

namespace StoreApi.Repositories
{
    public class DonorRepository : IDonorRepository
    {
        private readonly StoreContextDB _context;
        public DonorRepository(StoreContextDB context)
        {
            _context = context;
        }
        public async Task<List<Donor>> GetAllDonorsAsync()
        {
            return await _context.Donors.ToListAsync();
        }

        public async Task<Donor?> GetDonorByIdAsync(int id)
        {
            return await _context.Donors.FindAsync(id);
        }
        public async Task<Donor> CreateDonorAsync(Donor donor)
        {
            _context.Donors.Add(donor);
            await _context.SaveChangesAsync();
            return donor;
        }

        public async Task<Donor?> UpdateDonorAsync(int id, Donor donor)
        {
            var existingDonor = await _context.Donors.FindAsync(id);
            if (existingDonor == null)
            {
                return null;
            }
            existingDonor.Name = donor.Name;
            existingDonor.Email = donor.Email;
            existingDonor.Phone = donor.Phone;
            existingDonor.Address = donor.Address;
            await _context.SaveChangesAsync();
            return existingDonor;
        }
        public async Task<bool> DeleteDonorAsync(int id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null)
            {
                return false;
            }
            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
            return true;
        }
        //סינון לפי שם
        public async Task<List<Donor>> GetDonorsByNameAsync(string name)
        {
            return await _context.Donors
                .Where(d => d.Name.Contains(name))
                .ToListAsync();
        }
        //סינון לפי אימייל
        public async Task<List<Donor>> GetDonorsByEmailAsync(string email)
        {
            return await _context.Donors
                .Where(d => d.Email.Contains(email))
                .ToListAsync();
        }
        //סינון לפי מתנה
        public async Task<List<Donor>> GetDonorsByGiftIdAsync(int giftId)
        {
            return await _context.Donors
                .Where(d => d.Gifts.Any(g => g.Id == giftId))
                .ToListAsync();
        }


    }

}
