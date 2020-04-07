using System;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies the direction of the GPIO port
    /// </summary>
    public enum GPIODirection : byte
	{ 
		In = 0x00,    /*!< Input 0b000 */
		Out = 0x01,   /*!< Output 0b001 */
        ALT0  = 0x04,   /*!< Alternate function 0 0b100 */
        ALT1  = 0x05,   /*!< Alternate function 1 0b101 */
        ALT2  = 0x06,   /*!< Alternate function 2 0b110, */
        ALT3  = 0x07,   /*!< Alternate function 3 0b111 */
        ALT4  = 0x03,   /*!< Alternate function 4 0b011 */
        ALT5  = 0x02,   /*!< Alternate function 5 0b010 */
        MASK  = 0x07    /*!< Function select bits mask 0b111 */
	}
}