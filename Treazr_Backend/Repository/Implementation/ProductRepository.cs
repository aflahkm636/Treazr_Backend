using Microsoft.EntityFrameworkCore;
using Treazr_Backend.Data;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;

namespace Treazr_Backend.Repository.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _Context;

        public ProductRepository(AppDbContext context) :base(context) 
        {
            _Context = context;
        }
         
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _Context.Products
                .Include(p => p.Category)
                .Include(p=>p.Images)
                .Where(p=> p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();

        }

        public async Task<Product?> GetProductWithDetailsAsync(int id)
        {
            return await _Context.Products
                .Include(p=>p.Category)
                .Include(p=>p.Images)
                .FirstOrDefaultAsync(p=>p.Id == id && p.IsActive);
        }
    }
}
