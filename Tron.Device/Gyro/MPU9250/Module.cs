using System;

namespace Tron.Device.Gyro.MPU9250
{
    public class Module : BusDevice<Hardware.II2C>, IGyro
    {
        public Module()
        {
            this.BUS = new Hardware.I2C((byte)Register.MODULE_ADDRESS, Hardware.I2CClockDivider.CLOCK_DIVIDER_150);
            if(!this.Initialize())
            {
                throw new Exception("MPU9250 module initialize error");
            }
        }

        byte[] _buf = new byte[6];

        public Mode Mode
        {
            get;
            set;
        }

        private bool Initialize()
        {
            return this.BUS.ReadByte((byte)Register.WHO_AM_I) == 0x71;
        }

        public void Read()
        {
            this.BUS.Read(0x3B, _buf);
            var ax = (_buf[0] << 8) | _buf[1];
            var ay = (_buf[2] << 8) | _buf[3];
            var az = (_buf[4] << 8) | _buf[5];
            {
                this.BUS.Read(0x43, _buf);
                var gx = (_buf[0] << 8) | _buf[1];
                var gy = (_buf[2] << 8) | _buf[3];
                var gz = (_buf[4] << 8) | _buf[5];
            }

            {
                this.BUS.Read(0x43, _buf);
                var cx = (_buf[0] << 8) | _buf[1];
                var cy = (_buf[2] << 8) | _buf[3];
                var cz = (_buf[4] << 8) | _buf[5];
            }
            var cm = 0;
            for (int i = 0; i < 1000; i++)
            {
                var cx = (_buf[0] << 8) | _buf[1];
                var cy = (_buf[2] << 8) | _buf[3];
                var cz = (_buf[4] << 8) | _buf[5];
                cm += cx + cy + cz;
            }

        }

    }
}
