using System;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies the direction of the GPIO port
    /// </summary>
    [System.Security.SuppressUnmanagedCodeSecurity]
    class BCM2835
    {
        public static bool Init()
        {
            return bcm2835_init();
        }
        public static bool Dispose()
        {
            return bcm2835_close();
        }
        public static string Version
        {
            get => string.Format("C library for Broadcom BCM 2835(version: {0}.{1})", bcm2835_version() / 10000, bcm2835_version() % 100);
        }
        public static uint Regbase(byte regbase)
        {
            return bcm2835_regbase(regbase);
        }
        public static void SetDebug(bool isDebug)
        {
            bcm2835_set_debug(isDebug);
        }

        public static void Delay(int millis)
        {
            bcm2835_delay(millis);
        }

        public static void DelayMicroseconds(long micros)
        {
            bcm2835_delayMicroseconds(micros);
        }

        #region Basic functions from bcm2835 library
        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_init")]
        protected static extern bool bcm2835_init();

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_close")]
        protected static extern bool bcm2835_close();

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_version")]
        protected static extern uint bcm2835_version();

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_regbase")]
        protected static extern uint bcm2835_regbase(byte regbase);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_set_debug")]
        protected static extern void bcm2835_set_debug(bool isDebug);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_delay")]
        protected static extern void bcm2835_delay(int millis);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_delayMicroseconds")]
        protected static extern void bcm2835_delayMicroseconds(long micros);
        #endregion

    }
}