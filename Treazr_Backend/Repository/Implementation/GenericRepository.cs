using Microsoft.EntityFrameworkCore;
using Treazr_Backend.Data;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;

namespace Treazr_Backend.Repository.Implementation
{
    public class GenericRepository<T>:IGenericRepository<T> where T:BaseEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();

        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public virtual async Task<T> GetByIdAsync(int id)=>  await _dbSet.FindAsync(id);

        public async Task AddAsync(T Entity)
        {
            await _dbSet.AddAsync(Entity);
            await _context.SaveChangesAsync();

        }

        public async  Task UpdateAsync(T Entity)
        {
            _dbSet.Update(Entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
                if(entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
