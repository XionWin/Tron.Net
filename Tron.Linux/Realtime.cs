using System;
using System.Runtime.InteropServices;

namespace Tron.Linux
{
    public class Realtime
    {
        public static void SetRealtime()
        {
            set_realtime();
        }
        
        #region Realtime from librt.so
        [DllImport("librt.so", EntryPoint = "set_realtime")]
        protected static extern void set_realtime();
        #endregion
    }
}
