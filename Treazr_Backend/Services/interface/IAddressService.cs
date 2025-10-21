using Treazr_Backend.DTOs.OrderDto;

namespace Treazr_Backend.Services.interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDTO>> GetUserAddressesAsync(int userId);
        Task<AddressDTO> GetAddressByIdAsync(int id, int userId);
        Task<AddressDTO> CreateAddressAsync(AddressDTO dto, int userId);
        Task<AddressDTO> UpdateAddressAsync(int id, AddressDTO dto, int userId);
        Task<bool> DeleteAddressAsync(int id, int userId);
    }
}
