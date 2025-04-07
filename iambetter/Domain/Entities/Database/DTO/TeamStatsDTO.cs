using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Database.Projections
{
    public class TeamStatsDTO : IModelIdentity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }
        public TeamStatisticsResponse TeamStatistics { get; set; }
        public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateDateUtc { get; set; } = DateTime.UtcNow;
    }
}
