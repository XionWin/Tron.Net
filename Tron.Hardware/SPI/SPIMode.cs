using System;

namespace Tron.Hardware
{
    /// <summary>
    /// This is a cleaner way to write the Pin Status
    /// </summary>
    public enum SPIMode : byte
    {
        MODE0 = 0, /*!< CPOL = 0, CPHA = 0 */
        MODE1 = 1, /*!< CPOL = 0, CPHA = 1 */
        MODE2 = 2, /*!< CPOL = 1, CPHA = 0 */
        MODE3 = 3  /*!< CPOL = 1, CPHA = 1 */
    }
}