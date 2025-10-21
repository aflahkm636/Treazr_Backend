using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Models;

namespace Treazr_Backend.Repository.interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetUserAddressesAsync(int userId);
        Task<Address?> GetAddressByIdAsync(int id, int userId);
        Task<Address> CreateAddressAsync(Address address);
        Task<Address> UpdateAddressAsync(Address address);
        Task<bool> DeleteAddressAsync(int id, int userId);
        Task<Address> GetOrCreateAddressAsync(CreateOrderDTO dto, int userId);


    }
}
