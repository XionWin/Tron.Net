using System;

namespace Tron.Device.Gyro.MPU9250
{
    public partial class Module : Hardware.I2CDevice<Register>, IGyro
    {
        public Module()
            : base(new Hardware.I2C((byte)Register.MODULE_ADDRESS, Hardware.I2CClockDivider.CLOCK_DIVIDER_150))
        {
            this.Initiailze();
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
                    this.Initiailze();
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
                    this.Initiailze();
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
                    this.Initiailze();
                }
            }
        }

        // Function which accumulates gyro and accelerometer data after device initialization. It calculates the average
        // of the at-rest readings and then loads the resulting offsets into accelerometer and gyro bias registers.
        public (float ax, float ay, float az, float gx, float gy, float gz) Calibrate()
        {
            return this.calibrate();
        }
        
        public Core.Data.Vector3 Accel
        {
            get => this.ReadAccelData();
        }

        public Core.Data.Vector3 Gyro
        {
            get => this.ReadGyroData();
        }


    }
}
