using System;

namespace Tron.Device.Gyro.AK8963
{
    public enum Register : byte
    {
        MODULE_ADDRESS = 0x0C,
        WHO_AM_I = 0x00,
        CNTL = 0x0A,  // Power down (0000), single-measurement (0001), self-test (1000) and Fuse ROM (1111) modes on bits 3:0
        CNTL2 = 0x0B,  // Reset

        ASAX = 0x10,  // Fuse ROM x-axis sensitivity adjustment value
        ASAY = 0x11,  // Fuse ROM y-axis sensitivity adjustment value
        ASAZ = 0x12,  // Fuse ROM z-axis sensitivity adjustment value
        EXT_SENS_DATA_00 = 0x49,

        XOUT_L = 0x03,
        XOUT_H = 0x04,
        YOUT_L = 0x05,
        YOUT_H = 0x06,
        ZOUT_L = 0x07,
        ZOUT_H = 0x08,



    }
}
