using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Logger.Models
{
    public class LogModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("createTime")] public DateTime CreateTime { get; set; }

        [BsonElement("name")] public string Name { get; set; }

        [BsonElement("description")] public string Description { get; set; }

        [BsonElement("log")] public List<string> Log { get; set; }
    }

    public class LogListItem
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
