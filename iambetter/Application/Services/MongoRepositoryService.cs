using iambetter.Application.Services.Interfaces;
using iambetter.Data.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace iambetter.Application.Services
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

        public async Task<IEnumerable<T>> GetByFilterAsync(System.Linq.Expressions.Expression<Func<T, bool>> filter)
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
    }

    public static class MongoDbSettingsExtension
    {
        public static string GetCollectionNameForType(this MongoDbSettings settings, Type entity)
        {
            return entity.Name switch
            {
                "Team" => settings.TeamCollectionName,
                "League" => settings.LeagueCollectionName,
                "Match" => settings.MatchCollectionName,
                "FixtureProjection" => settings.MatchCollectionName,
                _ => throw new ArgumentException($"No collection name mapping for type {entity.Name}")

            };
        }
    }


}
