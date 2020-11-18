using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Logger.Models
{
    public class ArgumentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("createTime")] public DateTime CreateTime { get; set; }

        [BsonElement("name")] public string Name { get; set; }

        [BsonElement("schema")] public string Schema { get; set; }

        [BsonElement("content")] public string Content { get; set; }
    }

    public class ArgumentSnapshotModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ArgumentId { get; set; }

        [BsonElement("createTime")] public DateTime CreateTime { get; set; }

        [BsonElement("name")] public string Name { get; set; }

        [BsonElement("tag")] public string Tag { get; set; }

        [BsonElement("content")] public string Content { get; set; }
    }

    public class LogWithArgumentSnapshot
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("logId")] public string LogId { get; set; }

        [BsonElement("argumentSnapshotId")] public string ArgumentSnapshotId { get; set; }
    }

    public class ArgumentMeta
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
    }

    public class ArgumentSnapshotMeta
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
    }

    public class CreateArgumentRequest
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Content { get; set; }
    }

    public class CreateSnapshotRequest
    {
        public string Name { get; set; }
    }

    public class ArgumentUpdateRequest
    {
        public string Content { get; set; }
    }

    public class TagSnapshotRequest
    {
        public string Tag { get; set; }
    }
}