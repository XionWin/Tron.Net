using System;

namespace Tron.Device.Gyro
{
    public struct Quaternion
    {
        public Quaternion(double q1, double q2, double q3, double q4)
        {
            this.Q1 = q1;
            this.Q2 = q2;
            this.Q3 = q3;
            this.Q4 = q4;
        }

        public double Q1
        {
            get;
            private set;
        }
        public double Q2
        {
            get;
            private set;
        }
        public double Q3
        {
            get;
            private set;
        }
        public double Q4
        {
            get;
            private set;
        }
    }
}
