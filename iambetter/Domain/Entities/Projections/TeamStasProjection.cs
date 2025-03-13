using iambetter.Data.Interfaces;
using iambetter.Domain.Entities.API;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Projections
{
    public class TeamStasProjection : IModelIdentity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }

        public TeamStatisticsResponse Response { get; set; }
    }
}
