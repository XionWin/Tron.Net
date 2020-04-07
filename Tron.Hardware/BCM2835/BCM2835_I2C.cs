using System;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies the direction of the GPIO port
    /// </summary>
    [System.Security.SuppressUnmanagedCodeSecurity]
    public class BCM2835_I2C
    {
        public static bool Begin()
        {
            return bcm2835_i2c_begin();
        }
        public static void End()
        {
            bcm2835_i2c_end();
        }

        public static void SetSlaveAddress(byte addr)
        {
            bcm2835_i2c_setSlaveAddress(addr);
        }

        public static void SetClockDivider(I2CClockDivider divider)
        {
            bcm2835_i2c_setClockDivider(divider);
        }
        public static void SetBaudrate(uint baudrate)
        {
            bcm2835_i2c_set_baudrate(baudrate);
        }

        public static I2CReasonCodes Write(byte[] buf, int len)
        {
            return bcm2835_i2c_write(buf, len);
        }
        
        public static I2CReasonCodes Read(byte[] buf, int len)
        {
            return bcm2835_i2c_read(buf, len);
        }
        public static I2CReasonCodes ReadRegister(byte[] regaddr, byte[] buf, int len)
        {
            return bcm2835_i2c_read_register_rs(regaddr, buf, len);
        }
        public static I2CReasonCodes WriteReadRegister(byte[] cmds, int cmds_len, byte[] buf, int buf_len)
        {
            return bcm2835_i2c_write_read_rs(cmds, cmds_len, buf, buf_len);
        }

        #region I2C functions from bcm2835 library

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_i2c_begin")]
        protected static extern bool bcm2835_i2c_begin();

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_i2c_end")]
        protected static extern void bcm2835_i2c_end();

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_i2c_setSlaveAddress")]
        protected static extern void bcm2835_i2c_setSlaveAddress(byte addr);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_i2c_setClockDivider")]
        protected static extern void bcm2835_i2c_setClockDivider(I2CClockDivider divider);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_i2c_set_baudrate")]
        protected static extern void bcm2835_i2c_set_baudrate(uint baudrate);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_i2c_write")]
        protected static extern I2CReasonCodes bcm2835_i2c_write(byte[] buf, int len);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_i2c_read")]
        protected static extern I2CReasonCodes bcm2835_i2c_read(byte[] buf, int len);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_i2c_read_register_rs")]
        protected static extern I2CReasonCodes bcm2835_i2c_read_register_rs(byte[] regaddr, byte[] buf, int len);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_i2c_write_read_rs")]
        protected static extern I2CReasonCodes bcm2835_i2c_write_read_rs(byte[] cmds, int cmds_len, byte[] buf, int buf_len);

        #endregion

    }
}