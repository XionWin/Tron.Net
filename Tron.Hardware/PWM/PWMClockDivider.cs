using System;

namespace Tron.Hardware
{
    /// <summary>
    /// This is a cleaner way to write the Pin Status
    /// </summary>
    public enum PWMClockDivider : uint
    {    
		CLOCK_DIVIDER_2048  = 2048,    /*!< 2048 = 9.375kHz */
		CLOCK_DIVIDER_1024  = 1024,    /*!< 1024 = 18.75kHz */
		CLOCK_DIVIDER_512   = 512,     /*!< 512 = 37.5kHz */
		CLOCK_DIVIDER_256   = 256,     /*!< 256 = 75kHz */
		CLOCK_DIVIDER_128   = 128,     /*!< 128 = 150kHz */
		CLOCK_DIVIDER_64    = 64,      /*!< 64 = 300kHz */
		CLOCK_DIVIDER_32    = 32,      /*!< 32 = 600.0kHz */
		CLOCK_DIVIDER_16    = 16,      /*!< 16 = 1.2MHz */
		CLOCK_DIVIDER_8     = 8,       /*!< 8 = 2.4MHz */
		CLOCK_DIVIDER_4     = 4,       /*!< 4 = 4.8MHz */
		CLOCK_DIVIDER_2     = 2,       /*!< 2 = 9.6MHz, fastest you can get */
		CLOCK_DIVIDER_1     = 1        /*!< 1 = 4.6875kHz, same as divider 4096 */
    }
}