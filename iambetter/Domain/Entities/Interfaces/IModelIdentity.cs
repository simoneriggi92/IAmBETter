using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Interfaces
{
    public interface IModelIdentity
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }
    }
}
