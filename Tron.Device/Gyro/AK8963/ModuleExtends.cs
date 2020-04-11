using System;

namespace Tron.Device.Gyro.AK8963
{
    public partial class Module
    {
        private void Initiailze()
        {
            if (this.ID != 0x48)
            {
                throw new Exception("AK8963 module initialize error");
            }

        }

    }
}
