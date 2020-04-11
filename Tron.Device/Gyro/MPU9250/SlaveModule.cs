using System;

namespace Tron.Device.Gyro.MPU9250
{
    public partial class Module
    {
        private Mscale _mscale = Mscale.MFS_16BITS;
        private Mmode _mmode = Mmode.M_100Hz;

        private byte getSlaveID()
        {
            //  uint8_t c = readByte(AK8963_ADDRESS, WHO_AM_I_AK8963);  // Read WHO_AM_I register for MPU-9250
            this.WriteByte(Register.USER_CTRL, 0x20);    // Enable I2C Master mode  
            this.WriteByte(Register.I2C_MST_CTRL, 0x0D); // I2C configuration multi-master I2C 400KHz

            this.WriteByte(Register.I2C_SLV0_ADDR, (byte)Register.AK8963_ADDRESS | 0x80);    // Set the I2C slave address of AK8963 and set for read.
            this.WriteByte(Register.I2C_SLV0_REG, (byte)Register.AK8963_WHO_AM_I);           // I2C slave 0 register address from where to begin data transfer
            this.WriteByte(Register.I2C_SLV0_CTRL, 0x81);                     // Enable I2C and transfer 1 byte
            Hardware.Library.Delay(10);
            return this.ReadByte(Register.EXT_SENS_DATA_00);             // Read the WHO_AM_I byte
        }

        private void initiailzeSlave()
        {
            if (this.getSlaveID() != 0x48)
            {
                throw new Exception("Slave module initialize error");
            }

            // First extract the factory calibration for each magnetometer axis
            var rawData = new byte[3];  // x/y/z gyro calibration data stored here
            var magCalibration = new float[3];

            this.WriteByte(Register.I2C_SLV0_ADDR, (byte)Register.AK8963_ADDRESS);           // Set the I2C slave address of AK8963 and set for write.
            this.WriteByte(Register.I2C_SLV0_REG, (byte)Register.AK8963_CNTL2);              // I2C slave 0 register address from where to begin data transfer
            this.WriteByte(Register.I2C_SLV0_DO, 0x01);                       // Reset AK8963
            this.WriteByte(Register.I2C_SLV0_CTRL, 0x81);                     // Enable I2C and write 1 byte
            Hardware.Library.Delay(50);
            this.WriteByte(Register.I2C_SLV0_ADDR, (byte)Register.AK8963_ADDRESS);           // Set the I2C slave address of AK8963 and set for write.
            this.WriteByte(Register.I2C_SLV0_REG, (byte)Register.AK8963_CNTL);               // I2C slave 0 register address from where to begin data transfer
            this.WriteByte(Register.I2C_SLV0_DO, 0x00);                       // Power down magnetometer  
            this.WriteByte(Register.I2C_SLV0_CTRL, 0x81);                     // Enable I2C and write 1 byte
            Hardware.Library.Delay(50);
            this.WriteByte(Register.I2C_SLV0_ADDR, (byte)Register.AK8963_ADDRESS);           // Set the I2C slave address of AK8963 and set for write.
            this.WriteByte(Register.I2C_SLV0_REG, (byte)Register.AK8963_CNTL);               // I2C slave 0 register address from where to begin data transfer
            this.WriteByte(Register.I2C_SLV0_DO, 0x0F);                       // Enter fuze mode
            this.WriteByte(Register.I2C_SLV0_CTRL, 0x81);                     // Enable I2C and write 1 byte
            Hardware.Library.Delay(50);

            this.WriteByte(Register.I2C_SLV0_ADDR, (byte)Register.AK8963_ADDRESS | 0x80);    // Set the I2C slave address of AK8963 and set for read.
            this.WriteByte(Register.I2C_SLV0_REG, (byte)Register.AK8963_ASAX);               // I2C slave 0 register address from where to begin data transfer
            this.WriteByte(Register.I2C_SLV0_CTRL, 0x83);                     // Enable I2C and read 3 bytes
            Hardware.Library.Delay(50);

            this.Read(Register.EXT_SENS_DATA_00, rawData);        // Read the x-, y-, and z-axis calibration values
            magCalibration[0] = (float)(rawData[0] - 128) / 256.0f + 1.0f;        // Return x-axis sensitivity adjustment values, etc.
            magCalibration[1] = (float)(rawData[1] - 128) / 256.0f + 1.0f;
            magCalibration[2] = (float)(rawData[2] - 128) / 256.0f + 1.0f;

            this.WriteByte(Register.I2C_SLV0_ADDR, (byte)Register.AK8963_ADDRESS);           // Set the I2C slave address of AK8963 and set for write.
            this.WriteByte(Register.I2C_SLV0_REG, (byte)Register.AK8963_CNTL);               // I2C slave 0 register address from where to begin data transfer
            this.WriteByte(Register.I2C_SLV0_DO, 0x00);                       // Power down magnetometer  
            this.WriteByte(Register.I2C_SLV0_CTRL, 0x81);                     // Enable I2C and transfer 1 byte
            Hardware.Library.Delay(50);

            this.WriteByte(Register.I2C_SLV0_ADDR, (byte)Register.AK8963_ADDRESS);           // Set the I2C slave address of AK8963 and set for write.
            this.WriteByte(Register.I2C_SLV0_REG, (byte)Register.AK8963_CNTL);               // I2C slave 0 register address from where to begin data transfer 
                                                                                             // Configure the magnetometer for continuous read and highest resolution
                                                                                             // set Mscale bit 4 to 1 (0) to enable 16 (14) bit resolution in CNTL register,
                                                                                             // and enable continuous mode data acquisition Mmode (bits [3:0]), 0010 for 8 Hz and 0110 for 100 Hz sample rates
            this.WriteByte(Register.I2C_SLV0_DO, (byte)((byte)this._mscale << 4 | (byte)this.Mmode));        // Set magnetometer data resolution and sample ODR
            this.WriteByte(Register.I2C_SLV0_CTRL, 0x81);                     // Enable I2C and transfer 1 byte
            Hardware.Library.Delay(50);

            System.Console.WriteLine("{0}, {1}, {2}", magCalibration[0], magCalibration[1], magCalibration[2]);
        }

        private (float mbx, float mby, float mbz, float msx, float msy, float msz) calibrateSlave()
        {
            float[] dest1 = new float[3];
            float[] dest2 = new float[3];

            ushort sample_count = 0;
            var rawData_6 = new byte[6];
            var mag_temp = new short[3];
            var mag_max = new short[3];
            var mag_min = new short[3];

            var mag_bias = new int[3];


            var rawData_3 = new byte[3];
            var magCalibration = new float[3];

            float _mRes = this._mscale == Mscale.MFS_14BITS ? 10.0f * 4912.0f / 8190.0f : 10.0f * 4912.0f / 32760.0f;

            var mag_scale = new int[3];

            Hardware.Library.Delay(4000);

            // shoot for ~fifteen seconds of mag data
            if (this._mmode == Mmode.M_8Hz) sample_count = 128;  // at 8 Hz ODR, new mag data is available every 125 ms
            if (this._mmode == Mmode.M_100Hz) sample_count = 1500;  // at 100 Hz ODR, new mag data is available every 10 ms
            for (var i = 0; i < sample_count; i++)
            {
                this.WriteByte(Register.I2C_SLV0_ADDR, (byte)Register.AK8963_ADDRESS | 0x80);    // Set the I2C slave address of AK8963 and set for read.
                this.WriteByte(Register.I2C_SLV0_REG, (byte)Register.AK8963_XOUT_L);             // I2C slave 0 register address from where to begin data transfer
                this.WriteByte(Register.I2C_SLV0_CTRL, 0x87);                     // Enable I2C and read 7 bytes
                if (this._mmode == Mmode.M_8Hz) Hardware.Library.Delay(125);  // at 8 Hz ODR, new mag data is available every 125 ms
                if (this._mmode == Mmode.M_100Hz) Hardware.Library.Delay(10);   // at 100 Hz ODR, new mag data is available every 10 ms
                this.Read(Register.EXT_SENS_DATA_00, rawData_6);        // Read the x-, y-, and z-axis calibration values
                mag_temp[0] = (short)(((ushort)rawData_6[1] << 8) | rawData_6[0]);     // Turn the MSB and LSB into a signed 16-bit value
                mag_temp[1] = (short)(((ushort)rawData_6[3] << 8) | rawData_6[2]);     // Data stored as little Endian
                mag_temp[2] = (short)(((ushort)rawData_6[5] << 8) | rawData_6[4]);

                for (int j = 0; j < 3; j++)
                {
                    mag_max[j] = Math.Max(mag_temp[j], mag_max[j]);
                    mag_min[j] = Math.Min(mag_temp[j], mag_min[j]);
                }
            }
            // Get hard iron correction
            mag_bias[0] = (mag_max[0] + mag_min[0]) / 2;  // get average x mag bias in counts
            mag_bias[1] = (mag_max[1] + mag_min[1]) / 2;  // get average y mag bias in counts
            mag_bias[2] = (mag_max[2] + mag_min[2]) / 2;  // get average z mag bias in counts

            this.WriteByte(Register.I2C_SLV0_ADDR, (byte)Register.AK8963_ADDRESS | 0x80);    // Set the I2C slave address of AK8963 and set for read.
            this.WriteByte(Register.I2C_SLV0_REG, (byte)Register.AK8963_ASAX);               // I2C slave 0 register address from where to begin data transfer
            this.WriteByte(Register.I2C_SLV0_CTRL, 0x83);                     // Enable I2C and read 3 bytes
            Hardware.Library.Delay(50);
            this.Read(Register.EXT_SENS_DATA_00, rawData_3);          // Read the x-, y-, and z-axis calibration values
            magCalibration[0] = (float)(rawData_3[0] - 128) / 256.0f + 1.0f;        // Return x-axis sensitivity adjustment values, etc.
            magCalibration[1] = (float)(rawData_3[1] - 128) / 256.0f + 1.0f;
            magCalibration[2] = (float)(rawData_3[2] - 128) / 256.0f + 1.0f;

            dest1[0] = (float)mag_bias[0] * _mRes * magCalibration[0];  // save mag biases in G for main program
            dest1[1] = (float)mag_bias[1] * _mRes * magCalibration[1];
            dest1[2] = (float)mag_bias[2] * _mRes * magCalibration[2];

            // Get soft iron correction estimate
            mag_scale[0] = (mag_max[0] - mag_min[0]) / 2;  // get average x axis max chord length in counts
            mag_scale[1] = (mag_max[1] - mag_min[1]) / 2;  // get average y axis max chord length in counts
            mag_scale[2] = (mag_max[2] - mag_min[2]) / 2;  // get average z axis max chord length in counts

            float avg_rad = mag_scale[0] + mag_scale[1] + mag_scale[2];
            avg_rad /= 3.0f;

            dest2[0] = avg_rad / ((float)mag_scale[0]);
            dest2[1] = avg_rad / ((float)mag_scale[1]);
            dest2[2] = avg_rad / ((float)mag_scale[2]);

            Hardware.Library.Delay(200);
            return (dest1[0], dest1[1], dest1[2], dest2[0], dest2[1], dest2[2]);
        }

    }
}
