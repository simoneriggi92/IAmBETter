using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace iambetter.Application.Services.Database.Abstracts
{
    public abstract class BaseDataService<T> : IRepositoryService<T> where T : IModelIdentity
    {
        private IRepositoryService<T> _repositoryService;

        public BaseDataService(IRepositoryService<T> repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public async Task DeleteAsync(string id)
        {
            await _repositoryService.DeleteAsync(id);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return _repositoryService.GetAllAsync();
        }

        public Task<T> GetAsync(string id)
        {
            return _repositoryService.GetAsync(id);
        }

        public Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
        {
            return _repositoryService.GetByFilterAsync(filter);
        }

        public async Task InsertAsync(T entity)
        {
            await _repositoryService.InsertAsync(entity);
        }

        public async Task InsertAllAsync(IEnumerable<T> entities)
        {
            await _repositoryService.InsertAllAsync(entities);
        }

        public async Task UpdateAsync(string id, T entity)
        {
            await _repositoryService.UpdateAsync(id, entity);
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<T> filter, T replacementDocument, ReplaceOptions replaceOptions)
        {
            return await _repositoryService.ReplaceOneAsync(filter, replacementDocument, replaceOptions);
        }

        public Task<BulkWriteResult<T>> ReplaceManyAsync(IEnumerable<WriteModel<T>> replacementDocuments, BulkWriteOptions options)
        {
            return _repositoryService.ReplaceManyAsync(replacementDocuments, options);
        }
    }
}
