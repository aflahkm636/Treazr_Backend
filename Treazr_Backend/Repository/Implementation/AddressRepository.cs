using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Treazr_Backend.Data;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;

namespace Treazr_Backend.Repository.implementation
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AddressRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Address>> GetUserAddressesAsync(int userId)
        {
            return await _context.Addresses
        .Where(a => a.UserId == userId && !a.IsDeleted) 
                .ToListAsync();
        }

        public async Task<Address?> GetAddressByIdAsync(int id, int userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        public async Task<Address> CreateAddressAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> UpdateAddressAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAddressAsync(int id, int userId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null) return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Address> GetOrCreateAddressAsync(CreateOrderDTO dto, int userId)
        {
            if (dto.NewAddress != null)
            {
                var address = _mapper.Map<Address>(dto.NewAddress);
                address.UserId = userId;
                await _context.Addresses.AddAsync(address);
                await _context.SaveChangesAsync();
                return address;
            }
            else if (dto.AddressId.HasValue && dto.AddressId > 0)
            {
                var existing = await _context.Addresses.FindAsync(dto.AddressId.Value);
                return existing ?? throw new Exception("Address not found");
            }
            else
            {
                throw new Exception("Address is required");
            }
        }
    }
}
