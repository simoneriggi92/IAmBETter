using MongoDB.Driver;

namespace iambetter.Application.Services.Database.Interfaces
{
    public interface IRepositoryService<T>
    {
        public Task InsertAsync(T entity);
        public Task InsertAllAsync(IEnumerable<T> entities);

        public Task<T> GetAsync(string id);

        public Task<IEnumerable<T>> GetAllAsync();

        public Task UpdateAsync(string id, T entity);

        public Task DeleteAsync(string id);

        public Task<IEnumerable<T>> GetByFilterAsync(FilterDefinition<T> filter);

        public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<T> filter, T replacementDocument, ReplaceOptions replaceOptions);

        public Task<BulkWriteResult<T>> ReplaceManyAsync(IEnumerable<WriteModel<T>> replacementDocuments, BulkWriteOptions options);
    }
}
