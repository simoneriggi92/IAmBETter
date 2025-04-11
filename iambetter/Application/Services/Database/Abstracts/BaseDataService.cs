using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Interfaces;
using MongoDB.Driver;

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

        public Task<T> GetAsync(string propertyName, string id)
        {
            return _repositoryService.GetAsync(propertyName, id);
        }

        public Task<IEnumerable<T>> GetByFilterAsync(FilterDefinition<T> filter, ProjectionDefinition<T> projection = null)
        {
            return _repositoryService.GetByFilterAsync(filter, projection);
        }

         public Task<IEnumerable<T>> GetByFilterAsync(FilterDefinition<T> filter, SortDefinition<T> sortDefinition = null,  ProjectionDefinition<T> projection = null)
        {
            return _repositoryService.GetByFilterAsync(filter, sortDefinition, projection);
        }

        public async Task InsertAsync(T entity)
        {
            await _repositoryService.InsertAsync(entity);
        }

        public async Task InsertManyAsync(IEnumerable<T> entities)
        {
            await _repositoryService.InsertManyAsync(entities);
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
