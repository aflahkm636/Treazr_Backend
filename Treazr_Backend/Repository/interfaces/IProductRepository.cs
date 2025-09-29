using Treazr_Backend.Models;

namespace Treazr_Backend.Repository.interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<Product?> GetProductWithDetailsAsync(int id);
    }
}
