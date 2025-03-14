using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace iambetter.Application.Services.Database
{
    public class MongoRepositoryService<T> : IRepositoryService<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepositoryService(IMongoDatabase mongoDatabase, IOptions<MongoDbSettings> mongoDbSettings)
        {
            var collectionName = mongoDbSettings.Value.GetCollectionNameForType(typeof(T));
            _collection = mongoDatabase.GetCollection<T>(collectionName);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", id));
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public async Task<T> GetAsync(string id)
        {
            return await _collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetByFilterAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task InsertAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }
        public async Task InsertAllAsync(IEnumerable<T> entities)
        {
            await _collection.InsertManyAsync(entities);
        }


        public async Task UpdateAsync(string id, T entity)
        {
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", id), entity);
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<T> filter, T replacementDocument, ReplaceOptions replaceOptions)
        {
            return await _collection.ReplaceOneAsync(filter, replacementDocument, replaceOptions);
        }

        public async Task<BulkWriteResult<T>> ReplaceManyAsync(IEnumerable<ReplaceOneModel<T>> replacementDocuments, BulkWriteOptions options)
        {
            return await _collection.BulkWriteAsync(replacementDocuments, options);
        }

        public Task<BulkWriteResult<T>> ReplaceManyAsync(IEnumerable<WriteModel<T>> replacementDocuments, BulkWriteOptions options)
        {
            return _collection.BulkWriteAsync(replacementDocuments, options);
        }
    }

    public static class MongoDbSettingsExtension
    {
        public static string GetCollectionNameForType(this MongoDbSettings settings, Type entity)
        {
            return entity.Name switch
            {
                "Team" => settings.TeamCollectionName,
                "League" => settings.LeagueCollectionName,
                "MatchProjection" => settings.MatchCollectionName,
                "TeamStatsProjection" => settings.StatsCollectionName,
                _ => throw new ArgumentException($"No collection name mapping for type {entity.Name}")

            };
        }
    }


}
