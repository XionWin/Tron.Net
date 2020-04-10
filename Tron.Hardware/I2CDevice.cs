using System;

namespace Tron.Hardware
{
    public abstract class I2CDevice<RT> : BusDevice<II2C>
    where RT : Enum
    {
        protected I2CDevice(II2C i2c) : base(i2c)
        {

        }

        public abstract byte ID
        {
            get;
        }

        protected byte ReadByte(RT addr)
        {
            return this.BUS.ReadByte(Convert.ToByte(addr));
        }
        protected void WriteByte(byte addr, byte value)
        {
            if(this.BUS.WriteByte(addr, value) != Hardware.I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C WriteByte error");
            }
        }
        protected void WriteByte(RT addr, byte value)
        {
            if(this.BUS.WriteByte(Convert.ToByte(addr), value) != Hardware.I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C WriteByte error");
            }
        }

        protected void Read(RT addr, byte[] buf)
        {
            if(this.BUS.Read(Convert.ToByte(addr), buf) != Hardware.I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C WriteByte error");
            }
        }
        protected void Write(byte[] buf)
        {
            if(this.BUS.Write(buf) != Hardware.I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C WriteByte error");
            }
        }
        protected void ReadRegister(byte[] addr, byte[] buf)
        {
            if(this.BUS.ReadRegister(addr, buf) != Hardware.I2CReasonCodes.REASON_OK)
            {
                throw new Exception("I2C WriteByte error");
            }
        }
    }
}
