using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iambetter.Domain.Entities.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Database.Configuration
{
    public class SourceSettings : IModelIdentity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int[] Seasons { get; set; } = Array.Empty<int>();
        public int CurrentSeason => Seasons.OrderByDescending(x => x).First();
        public DateTime CreationDateUtc { get ;set; }
        public DateTime LastUpdateDateUtc { get ; set; }
    }
}