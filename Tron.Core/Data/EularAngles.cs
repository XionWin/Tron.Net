using System;

namespace Tron.Core.Data
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

        public override string ToString()
        {
            return string.Format("Pitch: {0} Roll: {1} Yaw: {2}", this.Pitch, this.Roll, this.Yaw);
        }
    }
}
