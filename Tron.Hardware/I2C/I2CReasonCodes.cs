using System;

namespace Tron.Hardware
{
    /// <summary>
    /// This is a cleaner way to write the Pin Status
    /// </summary>
    public enum I2CReasonCodes : uint
    {
        REASON_OK   	     = 0x00,      /*!< Success */
        REASON_ERROR_NACK    = 0x01,      /*!< Received a NACK */
        REASON_ERROR_CLKT    = 0x02,      /*!< Received Clock Stretch Timeout */
        REASON_ERROR_DATA    = 0x04       /*!< Not all data is sent / received */
    }
}