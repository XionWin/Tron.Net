using System;

namespace Tron.Device.Gyro
{
    public struct EularAngles
    {
        public EularAngles(double roll, double pitch, double yaw)
        {
            this.Roll = roll;
            this.Pitch = pitch;
            this.Yaw = yaw;
        }

        public double Roll
        {
            get;
            private set;
        }
        public double Pitch
        {
            get;
            private set;
        }
        public double Yaw
        {
            get;
            private set;
        }
    }
}
