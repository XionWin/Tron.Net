using System;

namespace Tron.Hardware
{
    /// <summary>
    /// This is a cleaner way to write the Pin Status
    /// </summary>
    public enum I2CClockDivider : uint
    {
        CLOCK_DIVIDER_2500   = 2500,      /*!< 2500 = 10us = 100 kHz */
        CLOCK_DIVIDER_626    = 626,       /*!< 622 = 2.504us = 399.3610 kHz */
        CLOCK_DIVIDER_250    = 250,       /*!< 150 = 100ns = 1 MHz */
        CLOCK_DIVIDER_200    = 200,       /*!< 150 = 80ns = 1.25 MHz */
        CLOCK_DIVIDER_150    = 150,       /*!< 150 = 60ns = 1.666 MHz (default at reset) */
        CLOCK_DIVIDER_148    = 148,       /*!< 148 = 59ns = 1.689 MHz */
    }
}