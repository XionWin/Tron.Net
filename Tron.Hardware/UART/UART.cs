using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    public interface IUART
    {
        #region Properties
        /// <summary>
        /// The currently assigned GPIO pin.
        /// </summary>
        UARTBaudRate BaudRate { get; set; }

        /// <summary>
        /// Gets the bit mask of this pin.
        /// </summary>
        UARTParity Parity { get; }

        /// <summary>
        /// Direction of the GPIO pin
        /// </summary>
        uint DataBits { get; }

        /// <summary>
        /// GPIO pull up-down resistor
        /// </summary>
        bool StopBit { get; }
        #endregion

        #region Functions
		bool Open(UARTBaudRate baudRate);
        void Close();
        void Write(byte value);
        void Write(byte[] data, int len);

        int ReadLength();
        byte Read();
        int Read(byte[] data, int len);

        #endregion


    }

    public partial class UART : IUART
    {
        #region Fields
        private int _fd = -1;
        private string _path;
        private UARTBaudRate _baudRate;
        private UARTParity _parity;
        private uint _dataBits;
        private bool _stopBit;

        #endregion


        internal string Path
        {
            get => _path;
        }

        #region Implement IUART
        public UARTBaudRate BaudRate
        {
            get => this._baudRate;
            set
            {
                if (this._baudRate != value) // Left to right eval ensures base class gets to check for disposed object access
                {
                    if (!BCM2835_UART.setbaudrate(_fd, value))
                    {
                        throw new Exception("Change baud rate failed. UART: " + this.Path);
                    }
                }
            }
        }

        public UARTParity Parity
        {
            get => this._parity;
            protected set => this._parity = value;
        }

        public uint DataBits
        {
            get => this._dataBits;
            protected set => this._dataBits = value;
        }

        public bool StopBit
        {
            get => this._stopBit;
            protected set => this._stopBit = value;
        }

        #endregion
    }

    public partial class UART
    {
        protected UART(string path)
        {
            this._path = path;
			this._baudRate = UARTBaudRate.B9600; 
            this._parity = UARTParity.NONE;
            this._dataBits = 8;
            this._stopBit = false;
        }

        private static IUART _Instance = null;
        public static IUART Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new UART(Parameters.UART_PATH);
                }
                return _Instance;
            }
        }

        #region Implement IGPIO Functions

        public bool Open(UARTBaudRate baudRate)
        {
			this._baudRate = baudRate; 
            _fd = BCM2835_UART.Open(this.Path, this.BaudRate);
            BCM2835_UART.Flush(_fd);
            return _fd > 0;
        }

        public void Close()
        {
            BCM2835_UART.Close(this._fd);
        }

        public void Write(byte value)
        {
            if (_fd <= 0)
                throw new Exception("Invalidate file description.");
            BCM2835_UART.Wirte(_fd, value);
            Hardware.Library.DelayMicroseconds(100);
        }

        public void Write(byte[] data, int len)
        {
            if (_fd <= 0)
                throw new Exception("Invalidate file description.");
            BCM2835_UART.Wirte(_fd, data, len);
            Hardware.Library.DelayMicroseconds(100);
        }
        public int ReadLength()
        {
            if (_fd <= 0)
                throw new Exception("Invalidate file description.");
            return BCM2835_UART.ReadLength(_fd);
        }
        public byte Read()
        {
            if (_fd <= 0)
                throw new Exception("Invalidate file description.");
            var r = BCM2835_UART.Read(_fd);
            Hardware.Library.DelayMicroseconds(100);
            return r;
        }
        public int Read(byte[] data, int len)
        {
            if (_fd <= 0)
                throw new Exception("Invalidate file description.");
            var r = BCM2835_UART.Read(_fd, data, len);
            Hardware.Library.DelayMicroseconds(100);
            return r;
        }

        #endregion


    }
}
