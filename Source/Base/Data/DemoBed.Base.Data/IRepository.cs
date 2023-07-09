using System.Linq.Expressions;

namespace DemoBed.Base.Data
{
    public interface IRepository<T> where T : class
    {
        T? GetById(int id);
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        IEnumerable<T?> GetAll();
        Task<IEnumerable<T?>> GetAllAsync(CancellationToken cancellationToken = default);
        IQueryable<T> GetAllQueryable();
        //IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        bool Add(T entity);
        Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        bool Update(T entity);
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        bool Remove(T entity);
        Task<bool> RemoveAsync(T entity, CancellationToken cancellationToken = default);
        bool RemoveRange(IEnumerable<T> entities);
        Task<bool> RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    }
}