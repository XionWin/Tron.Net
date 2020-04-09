using System;

namespace Tron.Device.MotorControl.PCA9685
{
    public enum Register : byte
    {
        MODULE_ADDRESS = 0x40,

        MODE1 = 0x00,
        MODE2 = 0x01,
        PRE_SCALE = 0xFE,

        

    }
}
