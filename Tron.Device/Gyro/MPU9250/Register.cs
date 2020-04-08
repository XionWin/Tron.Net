using System;

namespace Tron.Device.Gyro.MPU9250
{
    public enum Register: byte
    {
        MAG_ADDRESS = 0x0C,
        MAG_X_L = 0x03,
        MAG_X_H = 0x04,
        MAG_Y_L = 0x05,
        MAG_Y_H = 0x06,
        MAG_Z_L = 0x05,
        MAG_Z_H = 0x06,

    }
}
