using API.Models.Domain;
using System.Linq.Expressions;

namespace API.DAL
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> GetCountAll();
        Task<IEnumerable<T>> GetAsync(int page, int pageSize);
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> CreateManyAsync(IEnumerable<T> entities);
        Task BulkCopyAsync(Job job, IEnumerable<T> entities);
        Task<T> DeleteAsync(T entity);
        Task<T> DeleteByIdAsync(object id);
        Task<T> UpdateAsync(T entity);
        Task SaveAsync();
    }
}
