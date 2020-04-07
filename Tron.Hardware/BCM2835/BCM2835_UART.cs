using System;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies the direction of the GPIO port
    /// </summary>
    public class BCM2835_UART
    {
        public static int Open(string name, UARTBaudRate baudRate)
        {
            return serial_open(name, baudRate);
        }

        public static void Close(int fd)
        {
            serial_close(fd);
        }
        public static bool setbaudrate(int fd, UARTBaudRate baudRate)
        {
            return serial_setbaudrate(fd, baudRate);
        }

        public static void Wirte(int fd, byte value)
        {
            serial_writechar(fd, value);
        }

        public static void Wirte(int fd, byte[] data, int len)
        {
            serial_write(fd, data, len);
        }

        public static void Flush(int fd)
        {
            serial_flush(fd);
        }


        public static int ReadLength(int fd)
        {
            return serial_readlen(fd);
        }

        public static byte Read(int fd)
        {
            return serial_readchar(fd);
        }

        public static int Read(int fd, byte[] data, int len)
        {
            return serial_read(fd, data, len);
        }



        #region UART functions from bcm2835 library

        [DllImport("libserial.so", EntryPoint = "serial_open")]
        protected static extern int serial_open(string name, UARTBaudRate baudRate);

        [DllImport("libserial.so", EntryPoint = "serial_setbaudrate")]
        protected static extern bool serial_setbaudrate(int fd, UARTBaudRate baudRate);

        [DllImport("libserial.so", EntryPoint = "serial_close")]
        protected static extern void serial_close(int fd);

        [DllImport("libserial.so", EntryPoint = "serial_writechar")]
        protected static extern void serial_writechar(int fd, byte value);

        [DllImport("libserial.so", EntryPoint = "serial_write")]
        protected static extern void serial_write(int fd, byte[] data, int len);

        [DllImport("libserial.so", EntryPoint = "serial_flush")]
        protected static extern void serial_flush(int fd);

        [DllImport("libserial.so", EntryPoint = "serial_readlen")]
        protected static extern int serial_readlen(int fd);

        [DllImport("libserial.so", EntryPoint = "serial_readchar")]
        protected static extern byte serial_readchar(int fd);

        [DllImport("libserial.so", EntryPoint = "serial_read")]
        protected static extern int serial_read(int fd, byte[] data, int len);

        #endregion

    }
}