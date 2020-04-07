using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    public interface II2C
    {
        #region Properties

        byte SlaveAddress { get; set; }
        I2CClockDivider ClockDivider { get; set; }
        uint Baudrate { get; set; }

        #endregion

        #region Functions

        bool Begin();
        void End();

        byte ReadByte(byte address);
        I2CReasonCodes WriteByte(byte address, byte value);
        I2CReasonCodes Read(byte addr, byte[] buf);
        I2CReasonCodes Write(byte[] buf);

        I2CReasonCodes ReadRegister(byte[] regaddr, byte[] buf);
        I2CReasonCodes WriteReadRegister(byte[] cmds, byte[] buf);
        #endregion


    }

    public partial class I2C : II2C
    {
        #region Fields
        private byte _slaveAddress = 0x00;
        private I2CClockDivider _clockDivider = I2CClockDivider.CLOCK_DIVIDER_150;
        private uint _baudrate = 0;
        private byte[] _addr_buffer = new byte[1];
        private byte[] _byte_buffer = new byte[2];
        #endregion

        protected I2C()
        {

        }

        #region Implement II2C
        public byte SlaveAddress
        {
            get => this._slaveAddress;
            set
            {
                if (this._slaveAddress != value)
                {
                    BCM2835_I2C.SetSlaveAddress(value);
                }
            }
        }

        public I2CClockDivider ClockDivider
        {
            get => this._clockDivider;
            set
            {
                if (this._clockDivider != value)
                {
                    BCM2835_I2C.SetClockDivider(value);
                }
            }
        }

        public uint Baudrate
        {
            get => this._baudrate;
            set
            {
                if (this._baudrate != value)
                {
                    BCM2835_I2C.SetBaudrate(value);
                }
            }
        }

        #endregion


        private static II2C _Instance = null;
        public static II2C Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new I2C();
                    _Instance.Begin();
                    _Instance.ClockDivider = Hardware.I2CClockDivider.CLOCK_DIVIDER_150;
                }
                return _Instance;
            }
        }
    }

    partial class I2C
    {
        #region Functions
        public bool Begin()
        {
            return BCM2835_I2C.Begin();
        }
        public void End()
        {
            BCM2835_I2C.End();
        }


        public byte ReadByte(byte address)
        {
            this._addr_buffer[0] = address;
            if (BCM2835_I2C.ReadRegister(this._addr_buffer, this._byte_buffer, this._byte_buffer.Length) != I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C read byte error.");
            }
            return this._byte_buffer[0];
        }
        public I2CReasonCodes WriteByte(byte address, byte value)
        {
            this._byte_buffer[0] = address;
            this._byte_buffer[1] = value;
            return BCM2835_I2C.Write(this._byte_buffer, this._byte_buffer.Length);
        }

        public I2CReasonCodes Read(byte addr, byte[] buf)
        {
            this._addr_buffer[0] = addr;
            if (BCM2835_I2C.Write(this._addr_buffer, this._addr_buffer.Length) != I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C read error.");
            }
            return BCM2835_I2C.Read(buf, buf.Length);
        }
        public I2CReasonCodes Write(byte[] buf)
        {
            return BCM2835_I2C.Write(buf, buf.Length);
        }
        public I2CReasonCodes ReadRegister(byte[] regaddr, byte[] buf)
        {
            return BCM2835_I2C.ReadRegister(regaddr, buf, buf.Length);
        }

        public I2CReasonCodes WriteReadRegister(byte[] cmds, byte[] buf)
        {
            return BCM2835_I2C.WriteReadRegister(cmds, cmds.Length, buf, buf.Length);
        }

        #endregion

    }
}
