using System.Security.Policy;
using MongoDB.Driver;
using Logger.Models;
using Microsoft.Extensions.Configuration;
using Logger.Protos;
using MongoDB.Driver.Linq;

namespace Logger.Services
{
    public interface IArgumentsService
    {
        public ArgumentModel GetArguments();
        public void UpdateArguments(ArgumentModel siteSetting);
        public void InitArguments();
        public void ArchiveArguments(string logId, Configuration config);
        public Configuration GetArchivedArguments(string logId);
    }

    public class ArgumentsService : IArgumentsService
    {
        private readonly IMongoCollection<ArgumentModel> _siteSetting;
        private readonly IMongoCollection<ConfigurationModel> _archiveConfig;
        private readonly IMongoCollection<LogModel> _logCollection;

        public ArgumentsService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("OnlineLogger"));
            var database = client.GetDatabase("OnlineLogger");
            _siteSetting = database.GetCollection<ArgumentModel>("arguments");
            _archiveConfig = database.GetCollection<ConfigurationModel>("archiveConfig");
            _logCollection = database.GetCollection<LogModel>("logs");

            if (_siteSetting.CountDocuments(item => true) != 1)
            {
                InitArguments();
            }
        }

        public ArgumentModel GetArguments() =>
            _siteSetting.Find(item => true).ToList()[0];

        public void UpdateArguments(ArgumentModel siteSetting)
        {
            if (siteSetting.Id == null)
            {
                siteSetting.Id = GetArguments().Id;
            }

            _siteSetting.ReplaceOne(item => true, siteSetting);
        }

        public void InitArguments()
        {
            _siteSetting.DeleteMany(item => true);
            _siteSetting.InsertOne(new ArgumentModel
            {
                Arguments = ""
            });
        }

        public void ArchiveArguments(string logId, Configuration config)
        {
            var configuration = new ConfigurationModel
            {
                Id = null,
                LogId = logId,
                Config = config
            };
            if (_logCollection.Find(item => item.Id == logId).FirstOrDefault() == null)
            {
                return;
            }

            _archiveConfig.InsertOne(configuration);
        }

        public Configuration GetArchivedArguments(string logId)
        {
            var config = _archiveConfig.AsQueryable()
                .Where(item => item.LogId == logId)
                .FirstOrDefault();
            return config == null ? null : config.Config;
        }
    }
}