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

        public DateTime CreateTime { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class RecordModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string LodId { get; set; }
        public DateTime Time { get; set; }
        public int ThreadId { get; set; }
        public string Level { get; set; }
        public string Content { get; set; }
    }
}
