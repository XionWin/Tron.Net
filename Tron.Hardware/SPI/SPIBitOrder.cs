using System;

namespace Tron.Hardware
{
    /// <summary>
    /// This is a cleaner way to write the Pin Status
    /// </summary>
    public enum SPIBitOrder : byte
    {
 		LSBFIRST = 0, /*!< LSB First */
 		MSBFIRST = 1  /*!< MSB First */
    }
}