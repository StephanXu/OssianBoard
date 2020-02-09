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
                Arguments = @"{
                                ""serialPort"": {
                                    ""portName"": ""COM3"",
                                    ""baudrate"": 230400,
                                    ""parity"": ""NoParity"",
                                    ""dataBit"": 8,
                                    ""stopBit"": ""OneStopBit"",
                                    ""synchronize"": true,
                                    ""syncInterval"": 1
                                },
                                ""camera"": {
                                    ""deviceIndex"": 0,
                                    ""frameWidth"": 1280,
                                    ""frameHeight"": 720
                                },
                                ""testVideoSource"": {
                                    ""filename"": ""test.avi""
                                },
                                ""aimbot"": {
                                    ""maxShootRadius"": 120,
                                    ""enemyColor"": ""Red"",
                                    ""brightness"": 30,
                                    ""thresColor"": 50,
                                    ""lightBarMinArea"": 15,
                                    ""lightBarContourMinSolidity"": 0.5,
                                    ""lightBarEllipseMinAspectRatio"": 1.8,
                                    ""armorMaxAngleDiff"": 7.5,
                                    ""armorMaxHeightDiffRatio"": 0.6,
                                    ""armorMaxYDiffRatio"": 2,
                                    ""armorMinXDiffRatio"": 0.5,
                                    ""armorBigArmorRatio"": 4.2,
                                    ""armorSmallArmorRatio"": 2,
                                    ""armorMinAspectRatio"": 1,
                                    ""armorMaxAspectRatio"": 5.5,
                                    ""areaNormalizedBase"": 1000,
                                    ""sightOffsetNormalizedBase"": 200
                                },
                                ""poseSolver"": {
                                    ""offsetX"": 0,
                                    ""offsetZPitch"": 131,
                                    ""offsetYPitch"": 50,
                                    ""offsetYYaw"": -37,
                                    ""offsetZYaw"": 213,
                                    ""offsetYaw"": 0,
                                    ""offsetPitch"": 0.55,
                                    ""initV"": 20,
                                    ""initK"": 0.026,
                                    ""gravity"": 9.8
                                }
                            }"
            });
        }
    }
}