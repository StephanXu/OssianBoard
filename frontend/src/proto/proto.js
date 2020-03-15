/*eslint-disable block-scoped-var, id-length, no-control-regex, no-magic-numbers, no-prototype-builtins, no-redeclare, no-shadow, no-var, sort-vars*/
"use strict";

var $protobuf = require("protobufjs/light");

var $root = ($protobuf.roots["default"] || ($protobuf.roots["default"] = new $protobuf.Root()))
.addJSON({
  OssianConfig: {
    nested: {
      SerialPort: {
        fields: {
          portName: {
            rule: "required",
            type: "string",
            id: 1
          },
          baudrate: {
            rule: "required",
            type: "int32",
            id: 2
          },
          parity: {
            rule: "required",
            type: "Parity",
            id: 3
          },
          dataBit: {
            rule: "required",
            type: "int32",
            id: 4
          },
          stopBit: {
            rule: "required",
            type: "StopBit",
            id: 5
          },
          synchronize: {
            rule: "required",
            type: "bool",
            id: 6
          },
          syncInterval: {
            rule: "required",
            type: "int32",
            id: 7
          }
        },
        nested: {
          Parity: {
            values: {
              NoParity: 0,
              OddParity: 1,
              EvenParity: 2,
              MarkParity: 3
            }
          },
          StopBit: {
            values: {
              OneStopBit: 0,
              One5StopBits: 1,
              TwoStopBits: 2
            }
          }
        }
      },
      Camera: {
        fields: {
          deviceIndex: {
            rule: "required",
            type: "int32",
            id: 1
          },
          frameWidth: {
            rule: "required",
            type: "int32",
            id: 2
          },
          frameHeight: {
            rule: "required",
            type: "int32",
            id: 3
          }
        }
      },
      TestVideoSource: {
        fields: {
          filename: {
            rule: "required",
            type: "string",
            id: 1
          }
        }
      },
      Aimbot: {
        fields: {
          enemyColor: {
            rule: "required",
            type: "EnemyColor",
            id: 1
          },
          brightness: {
            rule: "required",
            type: "int32",
            id: 2
          },
          thresColor: {
            rule: "required",
            type: "int32",
            id: 3
          },
          lightBarMinArea: {
            rule: "required",
            type: "double",
            id: 4
          },
          lightBarContourMinSolidity: {
            rule: "required",
            type: "double",
            id: 5
          },
          lightBarEllipseMinAspectRatio: {
            rule: "required",
            type: "double",
            id: 6
          },
          armorMaxAngleDiff: {
            rule: "required",
            type: "double",
            id: 7
          },
          armorMaxHeightDiffRatio: {
            rule: "required",
            type: "double",
            id: 8
          },
          armorMaxYDiffRatio: {
            rule: "required",
            type: "double",
            id: 9
          },
          armorMinXDiffRatio: {
            rule: "required",
            type: "double",
            id: 10
          },
          armorBigArmorRatio: {
            rule: "required",
            type: "double",
            id: 11
          },
          armorSmallArmorRatio: {
            rule: "required",
            type: "double",
            id: 12
          },
          armorMinAspectRatio: {
            rule: "required",
            type: "double",
            id: 13
          },
          armorMaxAspectRatio: {
            rule: "required",
            type: "double",
            id: 14
          },
          areaNormalizedBase: {
            rule: "required",
            type: "double",
            id: 15
          },
          sightOffsetNormalizedBase: {
            rule: "required",
            type: "double",
            id: 16
          }
        },
        nested: {
          EnemyColor: {
            values: {
              Red: 0,
              Blue: 2
            }
          }
        }
      },
      PoseSolver: {
        fields: {
          cameraToGimbalX: {
            rule: "required",
            type: "double",
            id: 1
          },
          cameraToGimbalY: {
            rule: "required",
            type: "double",
            id: 2
          },
          cameraToGimbalZ: {
            rule: "required",
            type: "double",
            id: 3
          },
          barrelToGimbalY: {
            rule: "required",
            type: "double",
            id: 4
          },
          rotOverlapLen: {
            rule: "required",
            type: "double",
            id: 5
          },
          initV: {
            rule: "required",
            type: "double",
            id: 6
          },
          initK: {
            rule: "required",
            type: "double",
            id: 7
          },
          gravity: {
            rule: "required",
            type: "double",
            id: 8
          },
          scaleDist: {
            rule: "required",
            type: "double",
            id: 9
          }
        }
      },
      Chassis: {
        fields: {
          kTopWz: {
            rule: "required",
            type: "double",
            id: 1
          }
        }
      },
      Gimbal: {
        fields: {}
      },
      Gun: {
        fields: {
          kFricSpeed12: {
            rule: "required",
            type: "int32",
            id: 1
          },
          kFricSpeed15: {
            rule: "required",
            type: "int32",
            id: 2
          },
          kFricSpeed18: {
            rule: "required",
            type: "int32",
            id: 3
          },
          kFricSpeed30: {
            rule: "required",
            type: "int32",
            id: 4
          },
          kFeedNormalRPM: {
            rule: "required",
            type: "int32",
            id: 5
          },
          kFeedSemiRPM: {
            rule: "required",
            type: "int32",
            id: 6
          },
          kFeedBurstRPM: {
            rule: "required",
            type: "int32",
            id: 7
          },
          kFeedAutoRPM: {
            rule: "required",
            type: "int32",
            id: 8
          }
        }
      },
      PIDParams: {
        fields: {
          kP: {
            rule: "required",
            type: "double",
            id: 1
          },
          kI: {
            rule: "required",
            type: "double",
            id: 2
          },
          kD: {
            rule: "required",
            type: "double",
            id: 3
          },
          thOut: {
            rule: "required",
            type: "double",
            id: 4
          },
          thIOut: {
            rule: "required",
            type: "double",
            id: 5
          }
        }
      },
      Configuration: {
        fields: {
          serialPort: {
            rule: "required",
            type: "SerialPort",
            id: 1
          },
          camera: {
            rule: "required",
            type: "Camera",
            id: 2
          },
          testVideoSource: {
            rule: "required",
            type: "TestVideoSource",
            id: 3
          },
          aimbot: {
            rule: "required",
            type: "Aimbot",
            id: 4
          },
          poseSolver: {
            rule: "required",
            type: "PoseSolver",
            id: 5
          },
          chassis: {
            rule: "required",
            type: "Chassis",
            id: 6
          },
          gimbal: {
            rule: "required",
            type: "Gimbal",
            id: 7
          },
          gun: {
            rule: "required",
            type: "Gun",
            id: 8
          },
          pidWheelSpeed: {
            rule: "required",
            type: "PIDParams",
            id: 9
          },
          pidChassisAngle: {
            rule: "required",
            type: "PIDParams",
            id: 10
          },
          pidAngleEcdPitch: {
            rule: "required",
            type: "PIDParams",
            id: 11
          },
          pidAngleGyroPitch: {
            rule: "required",
            type: "PIDParams",
            id: 12
          },
          pidAngleSpeedPitch: {
            rule: "required",
            type: "PIDParams",
            id: 13
          },
          pidAngleEcdYaw: {
            rule: "required",
            type: "PIDParams",
            id: 14
          },
          pidAngleGyroYaw: {
            rule: "required",
            type: "PIDParams",
            id: 15
          },
          pidAngleSpeedYaw: {
            rule: "required",
            type: "PIDParams",
            id: 16
          },
          pidFricSpeed: {
            rule: "required",
            type: "PIDParams",
            id: 17
          },
          pidFeedSpeed: {
            rule: "required",
            type: "PIDParams",
            id: 18
          }
        }
      }
    }
  }
});

module.exports = $root;
