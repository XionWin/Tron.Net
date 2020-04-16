using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    public interface II2C
    {
        #region Properties

        byte SlaveAddress { get; }
        I2CClockDivider ClockDivider { get; set; }
        uint Baudrate { get; set; }

        #endregion

        #region Functions

        // bool Begin();
        // void End();

        bool ReadBit(byte address, byte bitNum);
        I2CReasonCodes WriteBit(byte address, byte bitNum, bool value);

        byte ReadBits(byte address, byte bitStart, byte len);
        I2CReasonCodes WriteBits(byte address, byte bitStart, byte len, byte value);

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
        private I2CClockDivider _clockDivider = I2CClockDivider.CLOCK_DIVIDER_250;
        private uint _baudrate = 0;
        private byte[] _addr_buffer = new byte[1];
        private byte[] _byte_buffer = new byte[2];
        #endregion

        // private static bool _isInited = false;
        public I2C(byte slaveAddress, I2CClockDivider clockDivider)
        {
            this.Begin();
            this.SlaveAddress = slaveAddress;
            this.ClockDivider = clockDivider;
        }

        #region Implement II2C
        public byte SlaveAddress
        {
            get => this._slaveAddress;
            private set
            {
                if (this._slaveAddress != value)
                {
                    BCM2835_I2C.SetSlaveAddress(value);
                    this._slaveAddress = value;
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
                    this._clockDivider = value;
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
                    this._baudrate = value;
                }
            }
        }
        #endregion
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

        public bool ReadBit(byte address, byte bitNum)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            this._addr_buffer[0] = address;
            if (BCM2835_I2C.ReadRegister(this._addr_buffer, this._byte_buffer, 1) != I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C read byte error.");
            }
            return (this._byte_buffer[0] & Convert.ToByte(1 << bitNum)) != 0;
        }
        public I2CReasonCodes WriteBit(byte address, byte bitNum, bool value)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            byte mask = (byte)~(1 << bitNum);

            var register_byte = Convert.ToByte(this.ReadByte(address) & mask);
            var new_byte = Convert.ToByte(register_byte | (value ? 1 : 0) << bitNum);

            this._byte_buffer[0] = address;
            this._byte_buffer[1] = new_byte;
            return BCM2835_I2C.Write(this._byte_buffer, this._byte_buffer.Length);
        }

        public byte ReadBits(byte address, byte bitStart, byte len)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            this._addr_buffer[0] = address;
            if (BCM2835_I2C.ReadRegister(this._addr_buffer, this._byte_buffer, 1) != I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C read byte error.");
            }

            byte mask = Convert.ToByte(((1 << len) - 1) << (bitStart - len + 1));
            var register_byte = Convert.ToByte(this.ReadByte(address) & mask);

            return Convert.ToByte(register_byte >> (bitStart - len + 1));
        }

        public I2CReasonCodes WriteBits(byte address, byte bitStart, byte len, byte value)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            byte mask = Convert.ToByte(((1 << len) - 1) << (bitStart - len + 1));

            var register_byte = Convert.ToByte(this.ReadByte(address) & mask);
            var new_byte = Convert.ToByte(register_byte | value << (bitStart - len + 1));

            this._byte_buffer[0] = address;
            this._byte_buffer[1] = new_byte;
            return BCM2835_I2C.Write(this._byte_buffer, this._byte_buffer.Length);
        }


        public byte ReadByte(byte address)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            this._addr_buffer[0] = address;
            if (BCM2835_I2C.ReadRegister(this._addr_buffer, this._byte_buffer, 1) != I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C read byte error.");
            }
            return this._byte_buffer[0];
        }
        public I2CReasonCodes WriteByte(byte address, byte value)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            this._byte_buffer[0] = address;
            this._byte_buffer[1] = value;
            return BCM2835_I2C.Write(this._byte_buffer, this._byte_buffer.Length);
        }

        public I2CReasonCodes Read(byte addr, byte[] buf)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            this._addr_buffer[0] = addr;
            if (BCM2835_I2C.Write(this._addr_buffer, this._addr_buffer.Length) != I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C read error.");
            }
            return BCM2835_I2C.Read(buf, buf.Length);
        }
        public I2CReasonCodes Write(byte[] buf)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            return BCM2835_I2C.Write(buf, buf.Length);
        }
        public I2CReasonCodes ReadRegister(byte[] regaddr, byte[] buf)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            return BCM2835_I2C.ReadRegister(regaddr, buf, buf.Length);
        }

        public I2CReasonCodes WriteReadRegister(byte[] cmds, byte[] buf)
        {
            BCM2835_I2C.SetSlaveAddress(this.SlaveAddress);
            BCM2835_I2C.SetClockDivider(this.ClockDivider);

            return BCM2835_I2C.WriteReadRegister(cmds, cmds.Length, buf, buf.Length);
        }

        #endregion

    }
}
