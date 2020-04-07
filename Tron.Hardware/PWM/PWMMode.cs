using System;

namespace Tron.Hardware
{
    /// <summary>
    /// This is a cleaner way to write the Pin Status
    /// </summary>
    public enum PWMMode : byte
    {    
		Balanced  = 0x00,
		MarkSpace  = 0x01,
    }
}