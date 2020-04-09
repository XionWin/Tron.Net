using System;

namespace Tron.Device.Gyro.MPU9250
{
    public enum Register : byte
    {
        MODULE_ADDRESS = 0x68,

        WHO_AM_I = 0x75,
        MAG_ADDRESS = 0x0C,
        MAG_X_L = 0x03,
        MAG_X_H = 0x04,
        MAG_Y_L = 0x05,
        MAG_Y_H = 0x06,
        MAG_Z_L = 0x05,
        MAG_Z_H = 0x06,

    }
}
