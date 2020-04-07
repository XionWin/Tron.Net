using System;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies the direction of the GPIO port
    /// </summary>
    public class Library
	{ 
		public static bool Init()
		{
			return Hardware.BCM2835.Init();
		}
		public static void Dispose()
		{
			Hardware.BCM2835.Dispose();
		}

		public static string Version
		{
			get => Hardware.BCM2835.Version;
		}
		public static uint Regbase(byte regbase)
		{
			return Hardware.BCM2835.Regbase(regbase);
		}
		public static void SetDebug(bool isDebug)
		{
			Hardware.BCM2835.SetDebug(isDebug);
		}

		public static void Delay(int millis)
		{
			Hardware.BCM2835.Delay(millis);
		}

		public static void DelayMicroseconds(long micros)
		{
			Hardware.BCM2835.DelayMicroseconds(micros);
		}

	}
}