using System;

namespace Tron.Device.Gyro.AK8963
{
    public partial class Module : Hardware.I2CDevice<Register>
    {
        public Module()
            : base(new Hardware.I2C((byte)Register.MODULE_ADDRESS, Hardware.I2CClockDivider.CLOCK_DIVIDER_626))
        {
            this.Initiailze();
        }

        public override byte ID
        {
            get => this.ReadByte(Register.WHO_AM_I);
        }


    }
}
