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
                                    ""enemyColor"": ""Red"",

                                    ""brightness"": 30,
                                    ""thresColor"": 50,

                                    ""lightBarMinArea"": 15.0,
                                    ""lightBarContourMinSolidity"": 0.5,
                                    ""lightBarEllipseMinAspectRatio"": 1.8,


                                    ""armorMaxAngleDiff"": 7.5,
                                    ""armorMaxHeightDiffRatio"": 0.6,
                                    ""armorMaxYDiffRatio"": 2.0,
                                    ""armorMinXDiffRatio"": 0.5,

                                    ""armorBigArmorRatio"": 4.2,
                                    ""armorSmallArmorRatio"": 2.0,
                                    ""armorMinAspectRatio"": 1.0,
                                    ""armorMaxAspectRatio"": 5.5,

                                    ""areaNormalizedBase"": 1000.0,
                                    ""sightOffsetNormalizedBase"": 200.0
                                },
                                ""poseSolver"": {
                                                ""cameraToGimbalX"": 0.0,
                                    ""cameraToGimbalY"": -40.7,
                                    ""cameraToGimbalZ"": 141.1,
                                    ""barrelToGimbalY"": 40.1,
                                    ""rotOverlapLen"": 100000.0,

                                    ""initV"": 20.0,
                                    ""initK"": 0.026,
                                    ""gravity"": 9.8,
                                    ""scaleDist"": 1.2
                                },
                                ""chassis"": {
                                    ""PIDWheelSpeed_Kp"": 15000,
                                    ""PIDWheelSpeed_Ki"": 10,
                                    ""PIDWheelSpeed_Kd"": 0,
                                    ""PIDWheelSpeed_Th_Out"": 16000,
                                    ""PIDWheelSpeed_Th_IOut"": 2000,

                                    ""PIDChassisAngle_Kp"": 40,
                                    ""PIDChassisAngle_Ki"": 0,
                                    ""PIDChassisAngle_Kd"": 0,
                                    ""PIDChassisAngle_Th_Out"": 6,
                                    ""PIDChassisAngle_Th_IOut"": 0.2,

                                    ""TOP_WZ"": 3
                                }
                            }"
            });
        }
    }
}