using System;

namespace Tron.Device.Gyro.MPU9250
{
    public enum ClockSource : byte
    {
        CLOCK_INTERNAL = 0x00,
        CLOCK_PLL_XGYRO = 0x01,
        CLOCK_PLL_YGYRO = 0x02,
        CLOCK_PLL_ZGYRO = 0x03,
        CLOCK_PLL_EXT32K = 0x04,
        CLOCK_PLL_EXT19M = 0x05,
        CLOCK_KEEP_RESET = 0x07,
    }
}
