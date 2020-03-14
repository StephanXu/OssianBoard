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
          kTopWz: {
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
            type: "int32",
            id: 1
          },
          kFricSpeed15: {
            type: "int32",
            id: 2
          },
          kFricSpeed18: {
            type: "int32",
            id: 3
          },
          kFricSpeed30: {
            type: "int32",
            id: 4
          },
          kFeedNormalRPM: {
            type: "int32",
            id: 5
          },
          kFeedSemiRPM: {
            type: "int32",
            id: 6
          },
          kFeedBurstRPM: {
            type: "int32",
            id: 7
          },
          kFeedAutoRPM: {
            type: "int32",
            id: 8
          }
        }
      },
      PIDWheelSpeed: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
          }
        }
      },
      PIDChassisAngle: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
          }
        }
      },
      PIDAngleEcdPitch: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
          }
        }
      },
      PIDAngleEcdYaw: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
          }
        }
      },
      PIDAngleGyroPitch: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
          }
        }
      },
      PIDAngleGyroYaw: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
          }
        }
      },
      PIDAngleSpeedPitch: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
          }
        }
      },
      PIDAngleSpeedYaw: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
          }
        }
      },
      PIDFricSpeed: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
          }
        }
      },
      PIDFeedSpeed: {
        fields: {
          kP: {
            type: "double",
            id: 1
          },
          kI: {
            type: "double",
            id: 2
          },
          kD: {
            type: "double",
            id: 3
          },
          thOut: {
            type: "double",
            id: 4
          },
          thIOut: {
            type: "double",
            id: 5
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
          },
          gimbal: {
            type: "Gimbal",
            id: 7
          },
          gun: {
            type: "Gun",
            id: 8
          },
          pidWheelSpeed: {
            type: "PIDWheelSpeed",
            id: 9
          },
          pidChassisAngle: {
            type: "PIDChassisAngle",
            id: 10
          },
          pidAngleEcdPitch: {
            type: "PIDAngleEcdPitch",
            id: 11
          },
          pidAngleGyroPitch: {
            type: "PIDAngleGyroPitch",
            id: 12
          },
          pidAngleSpeedPitch: {
            type: "PIDAngleSpeedPitch",
            id: 13
          },
          pidAngleEcdYaw: {
            type: "PIDAngleEcdYaw",
            id: 14
          },
          pidAngleGyroYaw: {
            type: "PIDAngleGyroYaw",
            id: 15
          },
          pidAngleSpeedYaw: {
            type: "PIDAngleSpeedYaw",
            id: 16
          },
          pidFricSpeed: {
            type: "PIDFricSpeed",
            id: 17
          },
          pidFeedSpeed: {
            type: "PIDFeedSpeed",
            id: 18
          }
        }
      }
    }
  }
});

module.exports = $root;
