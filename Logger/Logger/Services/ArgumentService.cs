using MongoDB.Driver;
using Logger.Models;
using Microsoft.Extensions.Configuration;

namespace Logger.Services
{
    public class ArgumentService
    {
        private readonly IMongoCollection<ArgumentModel> _siteSetting;

        public ArgumentService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("OnlineLogger"));
            var database = client.GetDatabase("OnlineLogger");
            _siteSetting = database.GetCollection<ArgumentModel>("arguments");

            if (_siteSetting.CountDocuments(item => true) != 1)
            {
                Init();
            }
        }

        public ArgumentModel Get() =>
            _siteSetting.Find(item => true).ToList()[0];

        public void Update(ArgumentModel siteSetting)
        {
            if (siteSetting.Id == null)
            {
                siteSetting.Id = Get().Id;
            }

            _siteSetting.ReplaceOne(item => true, siteSetting);
        }

        public void Init()
        {
            _siteSetting.DeleteMany(item => true);
            _siteSetting.InsertOne(new ArgumentModel
            {
                Arguments = ""
            });
        }
    }
}