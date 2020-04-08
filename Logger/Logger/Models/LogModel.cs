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
        public int RecordCount { get; set; }
    }

    public class RecordModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string LogId { get; set; }
        public DateTime Time { get; set; }
        public int ThreadId { get; set; }
        public string Level { get; set; }
        public string Content { get; set; }
    }

    public class VariableModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string LogId { get; set; }
        public string Name { get; set; }
    }

    public class DotModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string VariableId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string RecordId { get; set; }
        public int ZoomLevel { get; set; }
        public long Time { get; set; }
        public double Value { get; set; }
    }

    public class PlotModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string LogId { get; set; }
        public string Name { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Variables { get; set; }
    }

    public class DotRequest
    {
        public long Time { get; set; }
        public double Value { get; set; }
    }

    public class VariableRequest
    {
        public string Name { get; set; }
        public List<DotRequest> Dots { get; set; }
    }

    public class PlotRequest
    {
        public string Name { get; set; }
        public List<VariableRequest> Variables { get; set; }
    }

    public class IncrementRequest
    {
        public List<RecordModel> Records { get; set; }
        public List<PlotRequest> Plots { get; set; }
    }
}
