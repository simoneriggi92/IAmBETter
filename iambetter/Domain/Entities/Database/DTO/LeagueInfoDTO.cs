using iambetter.Domain.Entities.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Database.DTO
{
    public class LeagueInfoDTO : IModelIdentity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }

        public string? CurrentRound { get; set; }
        public int LeagueId { get; set; }
        public string Name { get; set; }
        public int Season { get; set; }
        public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateDateUtc { get; set; } = DateTime.UtcNow;
        public string? MaxRounds { get; set; }
    }
}
