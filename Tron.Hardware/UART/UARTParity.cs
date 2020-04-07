using System;

namespace Tron.Hardware
{
	/// <summary>
	/// This is a cleaner way to write the Pin Status
	/// </summary>
	public enum UARTParity : byte
	{
		NONE = 50,
		B75 = 75,
		B110 = 110,
		B134 = 134,
	}
}