using System;

namespace Tron.Device.Gyro.MPU9250
{
    public partial class Module : Hardware.I2CDevice<Register>, IGyro
    {
        public Module()
            : base(new Hardware.I2C((byte)Register.MODULE_ADDRESS, Hardware.I2CClockDivider.CLOCK_DIVIDER_150))
        {
        }

        public override byte ID
        {
            get => this.ReadByte(Register.WHO_AM_I);
        }

        public Mode Mode
        {
            get;
            set;
        }

        public Ascale Ascale
        {
            get => this._ascale;
            set
            {
                if(this._ascale != value)
                {
                    this._ascale = value;
                }
            }
        }

        public Gscale Gscale
        {
            get => this._gscale;
            set
            {
                if(this._gscale != value)
                {
                    this._gscale = value;
                }
            }
        }

        public Mscale Mscale
        {
            get => this._mscale;
            set
            {
                if(this._mscale != value)
                {
                    this._mscale = value;
                }
            }
        }

        public Mmode Mmode
        {
            get => this._mmode;
            set
            {
                if(this._mmode != value)
                {
                    this._mmode = value;
                }
            }
        }

        public byte SampleRate
        {
            get => this._sampleRate;
            set
            {
                if(this._sampleRate != value)
                {
                    this._sampleRate = value;
                }
            }
        }
        
        public Core.Data.Vector3 Accel
        {
            get => this.readAccelData();
        }

        public Core.Data.Vector3 Gyro
        {
            get => this.readGyroData();
        }

        public void Initiailze()
        {
            this.initiailze();
            this.initiailzeSlave();
        }

        public void Reset()
        {
            this.reset();
        }
        public (float ax, float ay, float az, float gx, float gy, float gz) Calibrate()
        {
            return this.calibrate();
        }
    }
}
