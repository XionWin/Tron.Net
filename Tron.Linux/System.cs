using System;
using System.Runtime.InteropServices;

namespace Tron.Linux
{
    public partial class System
    {
        public static void SetRealtime()
        {
            set_realtime();
        }
        public static void Sleep(uint us)
        {
            sleep_us(us);
        }
        
        #region Realtime from liblnx.so
        [DllImport("liblnx.so", EntryPoint = "set_realtime")]
        protected static extern void set_realtime();

        [DllImport("liblnx.so", EntryPoint = "sleep_us")]
        protected static extern void sleep_us(uint us);
        #endregion
    }
}
