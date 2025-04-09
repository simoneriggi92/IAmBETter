using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Interfaces;
using iambetter.Domain.Entities.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Database.Projections
{
    public class MatchDTO : IModelIdentity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }
        public int? Season { get; set; }
        public string Round { get; set; }
        // public FixtureInfo Fixture { get; set; }
        public TeamInfo Teams { get; set; }
        public IEnumerable<TeamStatisticsResponse> TeamStatistics { get; set; } = new List<TeamStatisticsResponse>();

        public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateDateUtc { get; set; } = DateTime.UtcNow;
        public string Result { get; set; } = string.Empty;
    }
}
