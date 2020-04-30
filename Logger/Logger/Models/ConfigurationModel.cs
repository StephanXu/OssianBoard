using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Logger.Protos;

namespace Logger.Models
{
    public class ConfigurationModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string LogId { get; set; }
        [BsonElement("config")] public Configuration Config { get; set; }
    }
}