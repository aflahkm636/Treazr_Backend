using Treazr_Backend.DTOs.CategoryDTO;
using Treazr_Backend.Models;

namespace Treazr_Backend.Repository.interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task AddAsync(CategoryDTO newCategory);
        Task<Category?> GetByNameAsync(string name);

    }

}
