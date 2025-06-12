using iambetter.Domain.Entities.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Database.Configuration;

public class AppConfigurationDTO: IModelIdentity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [System.Text.Json.Serialization.JsonIgnore]
    public string Id { get; set; }
    public DateTime CreationDateUtc { get; set; }
    public DateTime LastUpdateDateUtc { get; set; }
    public string APIKey { get; set; } = string.Empty;
}