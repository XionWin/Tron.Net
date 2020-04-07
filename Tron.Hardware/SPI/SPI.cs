using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    public interface ISPI
    {
        #region Properties
        SPIChipSelect CS { get; set; }

        SPIBitOrder BitOrder { get; set; }

        SPIClockDivider ClockDivider { get; set; }

        SPIMode Mode { get; set; }
        #endregion

        #region Functions
		bool Begin();
        void End();
        void SetChipSelectPolarity(SPIChipSelect cs, bool isActive);

        /// <summary>
        /// Write a value to the pin
        /// </summary>
        /// <param name="value">The value to write to the pin</param>
        void Write(byte value);
        void Write(byte[] data, int len);
        void WriteHalf(ushort value);

        /// <summary>
        /// Read a value from the pin
        /// </summary>
        /// <returns>The value read from the pin</returns>
        byte Transfer(byte value);
        void Transfer(byte[] data, int len);

        #endregion


    }

    public partial class SPI : ISPI
    {
        #region Fields
        private SPIChipSelect _cs;
        private SPIBitOrder _bitOrder;
        private SPIClockDivider _clockDivider;
        private SPIMode _mode;
        #endregion

        protected SPI(SPIChipSelect cs, SPIBitOrder bitOrder, SPIClockDivider clockDivider, SPIMode mode)
        {
            this._cs = cs;
            this._bitOrder = bitOrder;
            this._clockDivider = clockDivider;
            this._mode = mode;
        }



        #region Implement ISPI
        public SPIChipSelect CS
        {
            get => this._cs;
            set
            {
                if (this._cs != value)
                {
                    BCM2835_SPI.SetChipSelect(value);
                }
            }
        }

        public SPIBitOrder BitOrder
        {
            get => this._bitOrder;
            set
            {
                if (this._bitOrder != value)
                {
                    BCM2835_SPI.SetBitOrder(value);
                }
            }
        }
        public SPIClockDivider ClockDivider
        {
            get => this._clockDivider;
            set
            {
                if (this._clockDivider != value)
                {
                    BCM2835_SPI.SetClockDivider(value);
                }
            }
        }
        public SPIMode Mode
        {
            get => this._mode;
            set
            {
                if (this._mode != value)
                {
                    BCM2835_SPI.SetMode(value);
                }
            }
        }

        #endregion

        private static ISPI _Instance = null;
        public static ISPI Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new SPI(SPIChipSelect.CS_NONE, SPIBitOrder.LSBFIRST, SPIClockDivider.CLOCK_DIVIDER_1024, SPIMode.MODE0);
                    _Instance.Begin();
                }
                return _Instance;
            }
        }
    }

    partial class SPI
    {
        #region Functions

        public bool Begin()
        {
            return BCM2835_SPI.Begin();
        }

        public void End()
        {
            BCM2835_SPI.End();
        }

        public void SetChipSelectPolarity(SPIChipSelect cs, bool isActive)
        {
            BCM2835_SPI.SetChipSelectPolarity(cs, isActive);
        }

        public void Write(byte value)
        {
            BCM2835_SPI.Transfer(value);
        }
        public void Write(byte[] data, int len)
        {
            BCM2835_SPI.Write(data, len);
        }
        public void WriteHalf(ushort value)
        {
            BCM2835_SPI.Write(value);
        }

        public byte Transfer(byte value)
        {
            return BCM2835_SPI.Transfer(value);
        }
        public void Transfer(byte[] data, int len)
        {
            BCM2835_SPI.Transfer(data, len);
        }

        #endregion


    }
}
