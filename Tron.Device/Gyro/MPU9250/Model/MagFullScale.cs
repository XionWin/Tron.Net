using System;

namespace Tron.Device.Gyro.MPU9250
{
    public enum MagFullScale : byte
    {
        MFS_14BITS = 0,     // 0.6 mG per LSB
        MFS_16BITS = 1,     // 0.15 mG per LSB
    }
}
