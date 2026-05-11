using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Repositories;

namespace StoreApi.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _repository;
        public DonorService(IDonorRepository repository)
        {
            this._repository = repository;
        }
        public async Task<List<DonorDto>> GetAllDonorsAsync()
        {
            var donors = await _repository.GetAllDonorsAsync();
            return donors.Select(d => new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
                Address = d.Address
            }).ToList();
        }

        public async Task<DonorDtoById?> GetDonorByIdAsync(int id)
        {
            var donor = await _repository.GetDonorByIdAsync(id);
            if (donor == null)
            {
                return null;
            }
            return new DonorDtoById
            {
                Name = donor.Name,
                Email = donor.Email,
                Phone = donor.Phone,
                Address = donor.Address,
                //Gifts = donor.Gifts.Select(g => new Gift
                //{
                //    Id = g.Id,
                //    Name = g.Name,
                //    Description = g.Description,
                //    Value = g.Value
                //}).ToList()

            };
        }
        public async Task<DonorDto> CreateDonorAsync(CreateDonorDto createDonorDto)
        {
            var donor = new Models.Donor
            {
                Name = createDonorDto.Name,
                Email = createDonorDto.Email,
                Phone = createDonorDto.Phone,
                Address = createDonorDto.Address
            };
            var createdDonor = await _repository.CreateDonorAsync(donor);
            return new DonorDto
            {
                Id = createdDonor.Id,
                Name = createdDonor.Name,
                Email = createdDonor.Email,
                Phone = createdDonor.Phone,
                Address = createdDonor.Address
            };
        }

        public async Task<DonorDtoById?> UpdateDonorAsync(int id, UpdateDonorDto updateDonorDto)
        {
            var donor = new Donor
            {
                Name = updateDonorDto.Name,
                Email = updateDonorDto.Email,
                Phone = updateDonorDto.Phone,
                Address = updateDonorDto.Address
            };
            var updatedDonor = await _repository.UpdateDonorAsync(id, donor);
            if (updatedDonor == null)
            {
                return null;
            }
            return new DonorDtoById
            {
                Id = updatedDonor.Id,
                Name = updatedDonor.Name,
                Email = updatedDonor.Email,
                Phone = updatedDonor.Phone,
                Address = updatedDonor.Address,
                //Gifts = updatedDonor.Gifts.Select(g => new Gift
                //{
                //    Id = g.Id,
                //    Name = g.Name,
                //    Description = g.Description,
                //    Value = g.Value
                //}).ToList()
            };
        }
        public async Task<bool> DeleteDonorAsync(int id)
        {
            return await _repository.DeleteDonorAsync(id);
        }
        // חיפוש תורמים לפי שם
        public async Task<List<DonorDto>> GetDonorsByNameAsync(string name)
        {
            var donors = await _repository.GetDonorsByNameAsync(name);
            return donors.Select(d => new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
                Address = d.Address
            }).ToList();
        }
        //תן לי סינון של תורמים לפי מייל
        public async Task<List<DonorDto>> GetDonorsByEmailAsync(string email)
        {
            var donors = await _repository.GetDonorsByEmailAsync(email);
            return donors.Select(d => new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
                Address = d.Address
            }).ToList();
        }
        //סינון לפי מתנה
        public async Task<List<DonorDto>> GetDonorsByGiftIdAsync(int giftId)
        {
            var donors = await _repository.GetDonorsByGiftIdAsync(giftId);
            return donors.Select(d => new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
                Address = d.Address
            }).ToList();
        }
    }
}
