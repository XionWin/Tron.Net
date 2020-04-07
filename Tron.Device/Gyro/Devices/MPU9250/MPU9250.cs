using System;

namespace Tron.Device.Gyro
{
    public class MPU9250 : BusDevice<Hardware.II2C>, IGyro
    {
        private const byte ADDRESS = 0x68;
        public MPU9250(Hardware.II2C i2c) : base(i2c)
        {
            this.BUS.Begin();
            this.BUS.SlaveAddress = 0x68;
        }
        byte[] _buf = new byte[6];
        public void Read()
        {
            this.BUS.SlaveAddress = ADDRESS;

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

        }

        public event OnEularAnglesChangedHanlder OnEularAnglesChanged;
        public event OnQuaternionChangedHanlder OnQuaternionChanged;
    }
}
