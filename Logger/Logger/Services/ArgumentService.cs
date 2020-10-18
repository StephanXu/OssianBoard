using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using MongoDB.Driver;
using Logger.Models;
using Microsoft.Extensions.Configuration;
using Logger.Models;

namespace Logger.Services
{
    public interface IArgumentsService
    {
        public ArgumentModel CreateArgument(
            string name,
            string schema,
            string content);

        public IEnumerable<ArgumentMeta> ListArguments();
        public ArgumentModel GetSingleArgument(string argId);
        public IEnumerable<ArgumentSnapshotMeta> ListSnapshot(string argId);
        public ArgumentSnapshotModel GetSingleSnapshot(string snapshotId);
        public void UpdateArguments(string argId, ArgumentModel argument);
        public ArgumentSnapshotModel CreateSnapshot(string argId, string name);
        public void RemoveSnapshot(string snapshotId);
        public void BindSnapshotForLog(string logId, string snapshotId);
        public void UnbindSnapshotFromLogId(string logId);
        public void RemoveArguments(string argId);
        public ArgumentSnapshotModel GetSingleSnapshotFromLogId(string logId);
        public void TagSnapshot(string snapshotId, string tag);
    }

    public class ArgumentsService : IArgumentsService
    {
        private readonly IMongoCollection<ArgumentModel> _arguments;
        private readonly IMongoCollection<ArgumentSnapshotModel> _snapshots;
        private readonly IMongoCollection<LogWithArgumentSnapshot> _snapshotsWithLog;
        private readonly IMongoCollection<LogModel> _logCollection;

        public ArgumentsService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("DatabaseConnection"));
            var database = client.GetDatabase(config.GetConnectionString("DatabaseName"));
            _arguments = database.GetCollection<ArgumentModel>("arguments");
            _snapshots = database.GetCollection<ArgumentSnapshotModel>("argumentsSnapshots");
            _snapshotsWithLog = database.GetCollection<LogWithArgumentSnapshot>("argSnapRelWithLog");
            _logCollection = database.GetCollection<LogModel>("logs");
        }

        public ArgumentModel CreateArgument(
            string name,
            string schema,
            string content)
        {
            var arg = new ArgumentModel
            {
                Content = content,
                Schema = schema,
                CreateTime = DateTime.Now,
                Id = null,
                Name = name
            };

            _arguments.InsertOne(arg);
            return arg;
        }

        public IEnumerable<ArgumentMeta> ListArguments() =>
            _arguments.AsQueryable()
                .Select(item => new ArgumentMeta
                {
                    Id = item.Id,
                    CreateTime = item.CreateTime,
                    Name = item.Name
                });

        public ArgumentModel GetSingleArgument(string argId) =>
            _arguments.AsQueryable()
                .FirstOrDefault(argument => argument.Id == argId);

        public IEnumerable<ArgumentSnapshotMeta> ListSnapshot(string argId) =>
            _snapshots.AsQueryable()
                .Where(item => item.ArgumentId == argId)
                .OrderByDescending(item => item.CreateTime)
                .Select(item => new ArgumentSnapshotMeta
                {
                    Id = item.Id,
                    CreateTime = item.CreateTime,
                    Name = item.Name,
                    Tag = item.Tag
                });

        public ArgumentSnapshotModel GetSingleSnapshot(string snapshotId) =>
            _snapshots.AsQueryable()
                .FirstOrDefault(snapshot => snapshot.Id == snapshotId);

        public void UpdateArguments(string argId, ArgumentModel argument)
        {
            _arguments.ReplaceOne(item => item.Id == argId, argument);
        }

        public ArgumentSnapshotModel CreateSnapshot(string argId, string name)
        {
            var arg = _arguments.AsQueryable().FirstOrDefault(argument => argument.Id == argId);
            if (arg == null)
            {
                throw new ArgumentException("Argument not exists");
            }

            var snapshot = new ArgumentSnapshotModel
            {
                Id = null,
                ArgumentId = arg.Id,
                Content = arg.Content,
                CreateTime = DateTime.Now,
                Name = name,
                Tag = null
            };

            _snapshots.InsertOne(snapshot);

            return snapshot;
        }

        public void RemoveSnapshot(string snapshotId)
        {
            var snapshot = _snapshots.AsQueryable()
                .FirstOrDefault(item => item.Id == snapshotId);
            if (null == snapshot)
            {
                throw new Exception("Snapshot did not exist");
            }

            _snapshotsWithLog.DeleteMany(item => item.ArgumentSnapshotId == snapshotId);
            _snapshots.DeleteOne(item => item.Id == snapshotId);
        }

        public void BindSnapshotForLog(string logId, string snapshotId)
        {
            if (0 == _logCollection.AsQueryable().Count(log => log.Id == logId))
            {
                throw new Exception("Log not exists");
            }

            if (0 == _snapshots.AsQueryable().Count(snapshot => snapshot.Id == snapshotId))
            {
                throw new Exception("Argument not exists");
            }

            if (null != _snapshotsWithLog.AsQueryable().FirstOrDefault(item => item.LogId == logId))
            {
                throw new Exception("Log has already binded to a snapshot");
            }

            var relation = new LogWithArgumentSnapshot
            {
                ArgumentSnapshotId = snapshotId,
                LogId = logId,
                Id = null
            };
            _snapshotsWithLog.InsertOne(relation);
        }

        public void UnbindSnapshotFromLogId(string logId)
        {
            _snapshotsWithLog.DeleteMany(item => item.LogId == logId);
        }

        public void RemoveArguments(string argId)
        {
            var arg = _arguments.AsQueryable()
                .FirstOrDefault(item => item.Id == argId);
            if (null == arg)
            {
                throw new Exception("Argument did not exist");
            }

            var snapshotIds = _snapshots.AsQueryable()
                .Where(item => item.ArgumentId == argId)
                .Select(item => item.Id)
                .ToList();
            _snapshotsWithLog.DeleteMany(item => snapshotIds.Contains(item.ArgumentSnapshotId));
            _snapshots.DeleteMany(item => item.ArgumentId == argId);
            _arguments.DeleteOne(item => item.Id == argId);
        }

        public ArgumentSnapshotModel GetSingleSnapshotFromLogId(string logId)
        {
            var relation = _snapshotsWithLog.AsQueryable()
                .FirstOrDefault(item => item.LogId == logId);
            if (null == relation)
            {
                return null;
            }

            var snapshot = _snapshots.AsQueryable()
                .FirstOrDefault(item => item.Id == relation.ArgumentSnapshotId);
            return snapshot;
        }

        public void TagSnapshot(string snapshotId, string tag)
        {
            var snapshot = _snapshots.AsQueryable()
                .FirstOrDefault(item => item.Id == snapshotId);
            if (snapshot == null)
            {
                throw new Exception("Snapshot does not exist");
            }

            snapshot.Tag = tag;
            _snapshots.ReplaceOne(item => item.Id == snapshotId, snapshot);
        }
    }
}