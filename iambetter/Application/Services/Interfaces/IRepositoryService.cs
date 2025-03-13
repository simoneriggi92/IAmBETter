using System.Linq.Expressions;

namespace iambetter.Application.Services.Interfaces
{
    public interface IRepositoryService<T>
    {
        public Task InsertAsync(T entity);
        public Task InsertAllAsync(IEnumerable<T> entities);

        public Task<T> GetAsync(string id);

        public Task<IEnumerable<T>> GetAllAsync();

        public Task UpdateAsync(string id, T entity);

        public Task DeleteAsync(string id);

        public Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter);
    }
}
