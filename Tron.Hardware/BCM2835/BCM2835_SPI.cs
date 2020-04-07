using System;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies the direction of the GPIO port
    /// </summary>
    [System.Security.SuppressUnmanagedCodeSecurity]
    class BCM2835_SPI
    {
        public static bool Begin()
        {
            return bcm2835_spi_begin();
        }
        public static void End()
        {
            bcm2835_spi_end();
        }
        public static void SetBitOrder(SPIBitOrder order)
        {
            bcm2835_spi_setBitOrder(order);
        }
        public static void SetClockDivider(SPIClockDivider divider)
        {
            bcm2835_spi_setClockDivider(divider);
        }
        public static void SetSpeed(uint speed)
        {
            if (speed >= BCM2835_AUX_SPI_CLOCK_MIN && speed <= BCM2835_AUX_SPI_CLOCK_MAX)
            {
                bcm2835_spi_set_speed_hz(speed);
            }
            else
            {
                throw new ArgumentOutOfRangeException("BCM2835 SPI speed out of range.");
            }
        }
        public static void SetMode(SPIMode mode)
        {
            bcm2835_spi_setDataMode(mode);
        }
        public static void SetChipSelect(SPIChipSelect cs)
        {
            bcm2835_spi_chipSelect(cs);
        }
        public static void SetChipSelectPolarity(SPIChipSelect cs, bool isActive)
        {
            bcm2835_spi_setChipSelectPolarity(cs, isActive);
        }
        public static byte Transfer(byte value)
        {
            return bcm2835_spi_transfer(value);
        }
        public static void Transfer(byte[] sendBuf, byte[] receiveBuf, int len)
        {
            bcm2835_spi_transfernb(sendBuf, receiveBuf, len);
        }
        public static void Transfer(byte[] sharedBuf, int len)
        {
            bcm2835_spi_transfern(sharedBuf, len);
        }
        public static void Write(byte[] buf, int len)
        {
            bcm2835_spi_writenb(buf, len);
        }
        public static void Write(ushort value)
        {
            bcm2835_spi_write(value);
        }


        #region SPI functions from bcm2835 library
        private const uint BCM2835_AUX_SPI_CLOCK_MIN = 30500;       /*!< 30,5kHz */
        private const uint BCM2835_AUX_SPI_CLOCK_MAX = 125000000;   /*!< 125Mhz */

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_begin")]
        protected static extern bool bcm2835_spi_begin();

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_end")]
        protected static extern void bcm2835_spi_end();

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_setBitOrder")]
        protected static extern void bcm2835_spi_setBitOrder(SPIBitOrder order);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_setClockDivider")]
        protected static extern void bcm2835_spi_setClockDivider(SPIClockDivider divider);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_set_speed_hz")]
        protected static extern void bcm2835_spi_set_speed_hz(uint speed_hz);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_setDataMode")]
        protected static extern void bcm2835_spi_setDataMode(SPIMode mode);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_chipSelect")]
        protected static extern void bcm2835_spi_chipSelect(SPIChipSelect cs);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_setChipSelectPolarity")]
        protected static extern void bcm2835_spi_setChipSelectPolarity(SPIChipSelect cs, bool isActive);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_transfer")]
        protected static extern byte bcm2835_spi_transfer(byte value);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_transfernb")]
        protected static extern void bcm2835_spi_transfernb(byte[] sendBuf, byte[] receiveBuf, int len);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_transfern")]
        protected static extern void bcm2835_spi_transfern(byte[] sharedBuf, int len);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_writenb")]
        protected static extern void bcm2835_spi_writenb(byte[] buf, int len);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_spi_write")]
        protected static extern void bcm2835_spi_write(ushort data);

        #endregion

    }
}