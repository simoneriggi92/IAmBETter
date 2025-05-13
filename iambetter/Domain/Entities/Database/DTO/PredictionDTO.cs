using iambetter.Domain.Entities.Interfaces;
using iambetter.Domain.Entities.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Database.DTO
{
    public class PredictionDTO : IModelIdentity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }
        public Team AwayTeam { get; set; } = new Team();
        public Team HomeTeam { get; set; } = new Team();
        public string PredictedResult { get; set; } = string.Empty;
        public DateTime CreationDateUtc { get; set; }
        public DateTime LastUpdateDateUtc { get; set; }
        public DateTime MatchDate { get; set; }
        public string Round { get; set; }
    }
}
