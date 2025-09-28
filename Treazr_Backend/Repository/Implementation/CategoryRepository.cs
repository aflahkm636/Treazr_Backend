using Microsoft.EntityFrameworkCore;
using Treazr_Backend.Data;
using Treazr_Backend.DTOs.CategoryDTO;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;

namespace Treazr_Backend.Repository.Implementation
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task AddAsync(CategoryDTO newCategory)
        {
            throw new NotImplementedException();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }

}
