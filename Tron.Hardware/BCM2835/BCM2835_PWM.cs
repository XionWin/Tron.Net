using System;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
	/// <summary>
	/// Specifies the direction of the GPIO port
	/// </summary>
	[System.Security.SuppressUnmanagedCodeSecurity]
	class BCM2835_PWM
	{
		public static void SetClockDivider(PWMClockDivider divider)
		{
			bcm2835_pwm_set_clock(divider);
		}
		public static void SetMode(int channel, PWMMode mode, bool enabled)
		{
			bcm2835_pwm_set_mode(channel, mode, enabled);
		}
		public static void SetRange(int channel, int range)
		{
			bcm2835_pwm_set_range(channel, range);
		}
		public static void SetData(int channel, int data)
		{
			bcm2835_pwm_set_data(channel, data);
		}


#region PWM functions from bcm2835 library
		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_clock")]
		protected static extern void bcm2835_pwm_set_clock(PWMClockDivider divider);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_mode")]
		protected static extern void  bcm2835_pwm_set_mode(int channel, PWMMode mode, bool enabled);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_range")]
		protected static extern void bcm2835_pwm_set_range(int channel, int range);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_data")]
		protected static extern void bcm2835_pwm_set_data(int channel, int data);
		
#endregion
	}
}