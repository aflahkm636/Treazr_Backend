

using AutoMapper;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services.implementation
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDTO>> GetUserAddressesAsync(int userId)
        {
            var addresses = await _repository.GetUserAddressesAsync(userId);
            return _mapper.Map<IEnumerable<AddressDTO>>(addresses);
        }

        public async Task<AddressDTO> GetAddressByIdAsync(int id, int userId)
        {
            var address = await _repository.GetAddressByIdAsync(id, userId)
                ?? throw new Exception("Address not found");
            return _mapper.Map<AddressDTO>(address);
        }

        public async Task<AddressDTO> CreateAddressAsync(AddressDTO dto, int userId)
        {
            var address = _mapper.Map<Address>(dto);
            address.UserId = userId;

            var created = await _repository.CreateAddressAsync(address);
            return _mapper.Map<AddressDTO>(created);
        }

        public async Task<AddressDTO> UpdateAddressAsync(int id, AddressDTO dto, int userId)
        {
            var existing = await _repository.GetAddressByIdAsync(id, userId)
                ?? throw new Exception("Address not found");

            _mapper.Map(dto, existing);

            var updated = await _repository.UpdateAddressAsync(existing);
            return _mapper.Map<AddressDTO>(updated);
        }

        public async Task<bool> DeleteAddressAsync(int id, int userId)
        {
            return await _repository.DeleteAddressAsync(id, userId);
        }
    }
}
