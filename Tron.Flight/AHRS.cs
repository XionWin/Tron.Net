using System;

namespace Tron.Flight
{
    public class AHRS
    {
        public Core.Data.Quaternion Quaternion
        {
            get;
            private set;
        }
        public Core.Data.EularAngles EularAngles
        {
            get;
            private set;
        }
    }
}
