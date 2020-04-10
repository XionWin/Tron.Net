using System;

namespace Tron.Core.Data
{
    public struct Vector3
    {
        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double X
        {
            get;
            private set;
        }
        public double Y
        {
            get;
            private set;
        }
        public double Z
        {
            get;
            private set;
        }
    }
}
