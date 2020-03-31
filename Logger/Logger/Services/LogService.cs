using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DnsClient.Protocol;
using Logger.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Logger.Services
{
    public interface ILogService
    {
        public IEnumerable<RecordModel> GetLog(string logId);
        public Task<LogModel> CreateLog(string name, string description);
        public IEnumerable<LogModel> ListLogs();
        public Task<IEnumerable<RecordModel>> AddRecord(string logId, IEnumerable<string> record);
        public Task<LogModel> RemoveLog(string logId);
    }

    public class LogService : ILogService
    {
        private readonly IMongoCollection<LogModel> _logCollection;
        private readonly IMongoCollection<RecordModel> _recordCollection;

        public LogService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("OnlineLogger"));
            var database = client.GetDatabase("OnlineLogger");
            _logCollection = database.GetCollection<LogModel>("logs");
            _recordCollection = database.GetCollection<RecordModel>("records");
        }

        public IEnumerable<RecordModel> GetLog(string logId) =>
            _recordCollection.AsQueryable()
                .Where(item => item.LodId == logId)
                .AsEnumerable();

        public async Task<LogModel> CreateLog(string name, string description)
        {
            var log = new LogModel
            {
                CreateTime = DateTime.Now,
                Name = name,
                Description = description
            };
            await _logCollection.InsertOneAsync(log);
            return log.Id == null ? null : log;
        }

        public IEnumerable<LogModel> ListLogs() =>
            _logCollection.AsQueryable()
                .Where(item => true);

        public async Task<IEnumerable<RecordModel>> AddRecord(string logId, IEnumerable<string> record)
        {
            var log = (await _logCollection.FindAsync(item => item.Id == logId)).FirstOrDefault();
            var records = record.Select(item =>
            {
                var matches = Regex.Matches(item, @"\[.*?\]");
                var matchesStr =
                    matches.Select(item => item.ToString().Substring(1, item.Length - 2).Trim())
                        .ToList();
                return new RecordModel
                {
                    LodId = log.Id,
                    Time = DateTime.Parse(matchesStr[0]),
                    ThreadId = Int32.Parse(matchesStr[1]),
                    Level = matchesStr[2],
                    Content = item.Substring(matches[2].Index + matches[2].Length + 1).TrimEnd()
                };
            });
            var recordModels = records.ToList();
            await _recordCollection.InsertManyAsync(recordModels);
            return recordModels;
        }

        public async Task<LogModel> RemoveLog(string logId)
        {
            var log = await _logCollection.FindOneAndDeleteAsync(item => item.Id == logId);
            await _recordCollection.DeleteManyAsync(item => item.LodId == log.Id);
            return log;
        }
    }
}