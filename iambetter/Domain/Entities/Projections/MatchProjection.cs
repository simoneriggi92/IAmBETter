using iambetter.Data.Interfaces;
using iambetter.Domain.Entities.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Projections
{
    public class MatchProjection : IModelIdentity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }
        public int? Season { get; set; }
        public string Round { get; set; }
        public FixtureInfo Fixture { get; set; }
        public TeamInfo Teams { get; set; }

    }
}
