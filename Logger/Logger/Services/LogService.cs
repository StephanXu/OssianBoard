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
        public Task<LogModel> GetLog(string logId);
        public Task<LogModel> CreateLog(string name, string description);
        public IEnumerable<LogListItem> ListLogs();
        public Task<IEnumerable<RecordModel>> AddRecord(string logId, IEnumerable<string> record);
    }

    public class LogService : ILogService
    {
        private readonly IMongoCollection<LogModel> _logCollection;

        public LogService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("OnlineLogger"));
            _logCollection = client.GetDatabase("OnlineLogger").GetCollection<LogModel>("logs");
        }

        public async Task<LogModel> GetLog(string logId)
        {
            var log = (await _logCollection.FindAsync(item => item.Id == logId)).SingleOrDefault();
            return log;
        }

        public async Task<LogModel> CreateLog(string name, string description)
        {
            var log = new LogModel
            {
                CreateTime = DateTime.Now,
                Name = name,
                Description = description,
                Log = new List<RecordModel>()
            };
            await _logCollection.InsertOneAsync(log);
            return log.Id == null ? null : log;
        }

        public IEnumerable<LogListItem> ListLogs() =>
            _logCollection.AsQueryable()
                .Select(item => new LogListItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    CreateTime = item.CreateTime
                });

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
                    Time = DateTime.Parse(matchesStr[0]),
                    ThreadId = Int32.Parse(matchesStr[1]),
                    Level = matchesStr[2],
                    Content = item.Substring(matches.Last().Index + matches.Last().Length + 1).TrimEnd()
                };
            });
            log.Log.AddRange(records);
            await _logCollection.ReplaceOneAsync(item => item.Id == logId, log);
            return records;
        }
    }
}
