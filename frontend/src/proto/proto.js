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
            type: "string",
            id: 1
          },
          baudrate: {
            type: "int32",
            id: 2
          },
          parity: {
            type: "Parity",
            id: 3
          },
          dataBit: {
            type: "int32",
            id: 4
          },
          stopBit: {
            type: "StopBit",
            id: 5
          },
          synchronize: {
            type: "bool",
            id: 6
          },
          syncInterval: {
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
            type: "int32",
            id: 1
          },
          frameWidth: {
            type: "int32",
            id: 2
          },
          frameHeight: {
            type: "int32",
            id: 3
          }
        }
      },
      TestVideoSource: {
        fields: {
          filename: {
            type: "string",
            id: 1
          }
        }
      },
      Aimbot: {
        fields: {
          enemyColor: {
            type: "EnemyColor",
            id: 1
          },
          brightness: {
            type: "int32",
            id: 2
          },
          thresColor: {
            type: "int32",
            id: 3
          },
          lightBarMinArea: {
            type: "double",
            id: 4
          },
          lightBarContourMinSolidity: {
            type: "double",
            id: 5
          },
          lightBarEllipseMinAspectRatio: {
            type: "double",
            id: 6
          },
          armorMaxAngleDiff: {
            type: "double",
            id: 7
          },
          armorMaxHeightDiffRatio: {
            type: "double",
            id: 8
          },
          armorMaxYDiffRatio: {
            type: "double",
            id: 9
          },
          armorMinXDiffRatio: {
            type: "double",
            id: 10
          },
          armorBigArmorRatio: {
            type: "double",
            id: 11
          },
          armorSmallArmorRatio: {
            type: "double",
            id: 12
          },
          armorMinAspectRatio: {
            type: "double",
            id: 13
          },
          armorMaxAspectRatio: {
            type: "double",
            id: 14
          },
          areaNormalizedBase: {
            type: "double",
            id: 15
          },
          sightOffsetNormalizedBase: {
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
            type: "double",
            id: 1
          },
          cameraToGimbalY: {
            type: "double",
            id: 2
          },
          cameraToGimbalZ: {
            type: "double",
            id: 3
          },
          barrelToGimbalY: {
            type: "double",
            id: 4
          },
          rotOverlapLen: {
            type: "double",
            id: 5
          },
          initV: {
            type: "double",
            id: 6
          },
          initK: {
            type: "double",
            id: 7
          },
          gravity: {
            type: "double",
            id: 8
          },
          scaleDist: {
            type: "double",
            id: 9
          }
        }
      },
      Chassis: {
        fields: {
          PIDWheelSpeed_Kp: {
            type: "double",
            id: 1
          },
          PIDWheelSpeed_Ki: {
            type: "double",
            id: 2
          },
          PIDWheelSpeed_Kd: {
            type: "double",
            id: 3
          },
          PIDWheelSpeed_Th_Out: {
            type: "double",
            id: 4
          },
          PIDWheelSpeed_Th_IOut: {
            type: "double",
            id: 5
          },
          PIDChassisAngle_Kp: {
            type: "double",
            id: 6
          },
          PIDChassisAngle_Ki: {
            type: "double",
            id: 7
          },
          PIDChassisAngle_Kd: {
            type: "double",
            id: 8
          },
          PIDChassisAngle_Th_Out: {
            type: "double",
            id: 9
          },
          PIDChassisAngle_Th_IOut: {
            type: "double",
            id: 10
          },
          Top_Wz: {
            type: "double",
            id: 11
          }
        }
      },
      Configuration: {
        fields: {
          serialPort: {
            type: "SerialPort",
            id: 1
          },
          camera: {
            type: "Camera",
            id: 2
          },
          testVideoSource: {
            type: "TestVideoSource",
            id: 3
          },
          aimbot: {
            type: "Aimbot",
            id: 4
          },
          poseSolver: {
            type: "PoseSolver",
            id: 5
          },
          chassis: {
            type: "Chassis",
            id: 6
          }
        }
      }
    }
  }
});

module.exports = $root;
