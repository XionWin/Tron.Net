using System;

namespace Tron.Device.Gyro.MPU9250
{
    public enum Register : byte
    {
        MODULE_ADDRESS = 0x68,

        WHO_AM_I = 0x75,

        CONFIG = 0x1A,
        ACCEL_CONFIG = 0x1C,
        ACCEL_CONFIG_2 = 0x1D,
        GYRO_CONFIG = 0x1B,

        ACCEL_ADDRESS = 0xD0,
        ACCEL_X_H = 0x3B,
        ACCEL_X_L = 0x3C,
        ACCEL_Y_H = 0x3D,
        ACCEL_Y_L = 0x3E,
        ACCEL_Z_H = 0x3F,
        ACCEL_Z_L = 0x40,

        GYRO_ADDRESS = 0xD0,
        GYRO_X_H = 0x43,
        GYRO_X_L = 0x44,	
        GYRO_Y_H = 0x45,
        GYRO_Y_L = 0x46,
        GYRO_Z_H = 0x47,
        GYRO_Z_L = 0x48,


        MAG_ADDRESS = 0x18,
        MAG_X_L = 0x03,
        MAG_X_H = 0x04,
        MAG_Y_L = 0x05,
        MAG_Y_H = 0x06,
        MAG_Z_L = 0x07,
        MAG_Z_H = 0x08,

    }
}
