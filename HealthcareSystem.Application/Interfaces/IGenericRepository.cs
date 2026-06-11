namespace HealthcareSystem.Application.Interfaces
{
    // Defines the standard contract for data access operations
    public interface IGenericRepository<T> where T : class
    {
        // Exposes the query builder to allow advanced filtering and pagination in services
        IQueryable<T> GetQueryable();

        Task<T?> GetByIdAsync(Guid id);

        // IReadOnlyList is preferred for read-only data to optimize performance
        Task<IReadOnlyList<T>> GetAllAsync();

        Task AddAsync(T entity);

        // Finds a single entity based on a specific condition (e.g., finding a user by email)
        Task<T?> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        // Commits changes to the database
        Task SaveChangesAsync();

        // Update and Delete are not usually asynchronous in EF Core, 
        // as they only change the tracking state until SaveChanges is called
        void Update(T entity);
        void Delete(T entity);
    }
}