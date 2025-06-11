using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iambetter.Domain.Entities.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iambetter.Domain.Entities.Database.DTO
{
    public class TaskDTO: IModelIdentity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }
        public string TaskName { get; set; }
        public DateTime LastExecutionTime { get; set; }
        public DateTime CreationDateUtc { get ; set ; }
        public DateTime LastUpdateDateUtc { get ; set ; }
    }
}