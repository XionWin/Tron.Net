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

    }
}
