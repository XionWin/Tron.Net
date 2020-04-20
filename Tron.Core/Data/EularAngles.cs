using System;

namespace Tron.Core.Data
{
    public struct EularAngles
    {
        public EularAngles(double pitch, double roll, double yaw)
        {
            this.Pitch = pitch;
            this.Roll = roll;
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
            return string.Format
            (
                "Pitch: {0} Roll: {1} Yaw: {2}",
                this.Pitch.ToString("N2").PadLeft(5),
                this.Roll.ToString("N2").PadLeft(5),
                this.Yaw.ToString("N2").PadLeft(5)
            );
        }
    }
}
