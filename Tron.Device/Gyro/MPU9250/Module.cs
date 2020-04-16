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

        public AccelFullScale Ascale
        {
            get => this._afs;
            set
            {
                if(this._afs != value)
                {
                    this._afs = value;
                    this._aRes = get_aRes(this._afs);
                }
            }
        }

        public GyroFullScale Gscale
        {
            get => this._gfs;
            set
            {
                if(this._gfs != value)
                {
                    this._gfs = value;
                    this._gRes = get_gRes(this._gfs);
                }
            }
        }

        public MagFullScale Mscale
        {
            get => this._mfs;
            set
            {
                if(this._mfs != value)
                {
                    this._mfs = value;
                    this._mRes = get_mRes(this._mfs);
                }
            }
        }

        public float Ares
        {
            get => this._aRes;
        }
        public float Gres
        {
            get => this._gRes;
        }
        public float Mres
        {
            get => this._mRes;
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
        public Core.Data.Vector3 Mag
        {
            get => this.readMagData();
        }

        
        public Core.Data.Vector3 AccBias
        {
            get;
            private set;
        }
        public Core.Data.Vector3 GyroBias
        {
            get;
            private set;
        }
        
        public Core.Data.Vector3 MagCalibration
        {
            get;
            private set;
        } = new Core.Data.Vector3(1, 1, 1);
        public Core.Data.Vector3 MagBias
        {
            get;
            private set;
        }
        public Core.Data.Vector3 MagScale
        {
            get;
            private set;
        } = new Core.Data.Vector3(1, 1, 1);

        public void Initiailze()
        {
            this.initiailze();
        }

        public void InitiailzeSlave()
        {
            this.initiailzeSlave();
        }

        public void Calibrate()
        {
            this.calibrate();
        }
        public void CalibrateSlave()
        {
            this.calibrateSlave();
        }
    }
}
