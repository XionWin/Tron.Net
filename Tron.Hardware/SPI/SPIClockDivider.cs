using System;

namespace Tron.Hardware
{
    /// <summary>
    /// This is a cleaner way to write the Pin Status
    /// </summary>
    public enum SPIClockDivider : uint
    {
        CLOCK_DIVIDER_65536 = 0,     /*!< 65536 = 3.814697260kHz on Rpi2, 6.1035156kHz on RPI3 */
        CLOCK_DIVIDER_32768 = 32768, /*!< 32768 = 7.629394531kHz on Rpi2, 12.20703125kHz on RPI3 */
        CLOCK_DIVIDER_16384 = 16384, /*!< 16384 = 15.25878906kHz on Rpi2, 24.4140625kHz on RPI3 */
        CLOCK_DIVIDER_8192 = 8192,   /*!< 8192 = 30.51757813kHz on Rpi2, 48.828125kHz on RPI3 */
        CLOCK_DIVIDER_4096 = 4096,   /*!< 4096 = 61.03515625kHz on Rpi2, 97.65625kHz on RPI3 */
        CLOCK_DIVIDER_2048 = 2048,   /*!< 2048 = 122.0703125kHz on Rpi2, 195.3125kHz on RPI3 */
        CLOCK_DIVIDER_1024 = 1024,   /*!< 1024 = 244.140625kHz on Rpi2, 390.625kHz on RPI3 */
        CLOCK_DIVIDER_512 = 512,     /*!< 512 = 488.28125kHz on Rpi2, 781.25kHz on RPI3 */
        CLOCK_DIVIDER_256 = 256,     /*!< 256 = 976.5625kHz on Rpi2, 1.5625MHz on RPI3 */
        CLOCK_DIVIDER_128 = 128,     /*!< 128 = 1.953125MHz on Rpi2, 3.125MHz on RPI3 */
        CLOCK_DIVIDER_64 = 64,       /*!< 64 = 3.90625MHz on Rpi2, 6.250MHz on RPI3 */
        CLOCK_DIVIDER_32 = 32,       /*!< 32 = 7.8125MHz on Rpi2, 12.5MHz on RPI3 */
        CLOCK_DIVIDER_16 = 16,       /*!< 16 = 15.625MHz on Rpi2, 25MHz on RPI3 */
        CLOCK_DIVIDER_8 = 8,         /*!< 8 = 31.25MHz on Rpi2, 50MHz on RPI3 */
        CLOCK_DIVIDER_4 = 4,         /*!< 4 = 62.5MHz on Rpi2, 100MHz on RPI3. Dont expect this speed to work reliably. */
        CLOCK_DIVIDER_2 = 2,         /*!< 2 = 125MHz on Rpi2, 200MHz on RPI3, fastest you can get. Dont expect this speed to work reliably.*/
        CLOCK_DIVIDER_1 = 1          /*!< 1 = 3.814697260kHz on Rpi2, 6.1035156kHz on RPI3, same as 0/65536 */
    }
}