using FastMember;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.DAL
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext context;
        protected readonly DbSet<T> dbSet;

        public RepositoryBase(ApplicationDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<int> GetCountAll()
        {
            return await dbSet.CountAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(int page, int pageSize)
        {
            return await dbSet.Skip(page * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            var entity = await dbSet.FindAsync(id);

            if (entity is null)
            {
                throw new KeyNotFoundException();
            }

            return entity;
        }

        public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> CreateAsync(T entity)
        {
            var newEntity = await dbSet.AddAsync(entity);

            return newEntity.Entity;
        }

        public async Task<IEnumerable<T>> CreateManyAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
            
            return entities;
        }

        public async Task BulkCopyAsync(IEnumerable<T> entities)
        {
            using (var reader = ObjectReader.Create(entities))
            using (var bulkCopy = new SqlBulkCopy(context.Database.GetDbConnection().ConnectionString))
            {
                bulkCopy.BulkCopyTimeout = 600;
                bulkCopy.DestinationTableName = dbSet.EntityType.GetTableName();
                await bulkCopy.WriteToServerAsync(reader);
            }
        }

        public async Task<T> DeleteAsync(T entity)
        {
            await Task.Run(() => dbSet.Remove(entity));

            return entity;
        }

        public async Task<T> DeleteByIdAsync(object id)
        {
            var entity = await dbSet.FindAsync(id);

            if (entity is null)
            {
                throw new KeyNotFoundException();
            }

            return await DeleteAsync(entity);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            await Task.Run(() => dbSet.Update(entity));

            return entity;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
