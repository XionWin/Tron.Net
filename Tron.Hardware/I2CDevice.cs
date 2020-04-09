using System;

namespace Tron.Hardware
{
    public abstract class I2CDevice<RT> : BusDevice<II2C>
    where RT : Enum
    {
        protected I2CDevice(II2C i2c) : base(i2c)
        {

        }

        protected byte ReadByte(RT addr)
        {
            return this.BUS.ReadByte(Convert.ToByte(addr));
        }
        protected Hardware.I2CReasonCodes WriteByte(byte addr, byte value)
        {
            return this.BUS.WriteByte(addr, value);
        }
        protected Hardware.I2CReasonCodes WriteByte(RT addr, byte value)
        {
            return this.BUS.WriteByte(Convert.ToByte(addr), value);
        }
        protected Hardware.I2CReasonCodes WriteByte(RT addr, RT value)
        {
            return this.BUS.WriteByte(Convert.ToByte(addr), Convert.ToByte(value));
        }

        protected Hardware.I2CReasonCodes Read(RT addr, byte[] buf)
        {
            return this.BUS.Read(Convert.ToByte(addr), buf);
        }
        protected Hardware.I2CReasonCodes Write(byte[] buf)
        {
            return this.BUS.Write(buf);
        }
        protected Hardware.I2CReasonCodes ReadRegister(byte[] addr, byte[] buf)
        {
            return this.BUS.ReadRegister(addr, buf);
        }
    }
}
