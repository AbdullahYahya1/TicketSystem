using Microsoft.EntityFrameworkCore;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.IRepositories;

namespace TicketSystem.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly TicketSystemDbContext _db;
        internal DbSet<T> _dbSet;
        public Repository(TicketSystemDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }
        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}

