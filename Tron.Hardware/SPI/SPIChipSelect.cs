using System;

namespace Tron.Hardware
{
    /// <summary>
    /// This is a cleaner way to write the Pin Status
    /// </summary>
    public enum SPIChipSelect : byte
    {  
      CS0 = 0,    /*!< Chip Select 0 */
      CS1 = 1,    /*!< Chip Select 1 */
      CS2 = 2,    /*!< Chip Select 2 (ie pins CS1 and CS2 are asserted) */
      CS_NONE = 3 /*!< No CS, control it yourself */
    }
}