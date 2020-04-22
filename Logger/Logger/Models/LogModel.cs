using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
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

    [MessagePackObject]
    public class RecordModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Key("id")]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [Key("logId")]
        public string LogId { get; set; }

        [Key("time")] public DateTime Time { get; set; }
        [Key("threadId")] public int ThreadId { get; set; }
        [Key("level")] public string Level { get; set; }
        [Key("content")] public string Content { get; set; }
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

    [MessagePackObject]
    public class DotRequest
    {
        [Key("time")] public long Time { get; set; }
        [Key("value")] public double Value { get; set; }
    }

    [MessagePackObject]
    public class VariableRequest
    {
        [Key("name")] public string Name { get; set; }
        [Key("dots")] public List<DotRequest> Dots { get; set; }
    }

    [MessagePackObject]
    public class PlotRequest
    {
        [Key("name")] public string Name { get; set; }
        [Key("variables")] public List<VariableRequest> Variables { get; set; }
    }

    [MessagePackObject]
    public class IncrementRequest
    {
        [Key("records")] public List<RecordModel> Records { get; set; }
        [Key("plots")] public List<PlotRequest> Plots { get; set; }
    }

    public class RawLogNavigator
    {
        public int Page { get; set; }
        public IEnumerable<RecordModel> Records { get; set; }
    }

    public class UnwindResult
    {
        public bool Id { get; set; }
        public DateTime RecordTime { get; set; }
        public int Index { get; set; }
    }
}