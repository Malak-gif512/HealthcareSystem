using HealthcareSystem.Application.Interfaces;
using HealthcareSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HealthcareSystem.Infrastructure.Repositories
{
    // Generic implementation of the repository pattern using Entity Framework Core
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetQueryable()
        {
            // AsQueryable allows EF Core to translate LINQ directly into optimized SQL queries
            return _context.Set<T>().AsQueryable();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task<T?> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            // Note: If the entity inherits from BaseEntity, 
            // the soft delete logic will be handled before SaveChanges
            _context.Set<T>().Remove(entity);
        }
    }
}