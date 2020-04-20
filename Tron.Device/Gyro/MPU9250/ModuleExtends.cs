using System;

namespace Tron.Device.Gyro.MPU9250
{
    public partial class Module
    {
        private const AccelFullScale AFS_DEFAULT_VALUE = AccelFullScale.AFS_8G;
        private const GyroFullScale GFS_DEFAULT_VALUE = GyroFullScale.GFS_2000DPS;
        private AccelFullScale _afs = AFS_DEFAULT_VALUE;
        private GyroFullScale _gfs = GFS_DEFAULT_VALUE;
        private byte _sampleRate = 0x00;

        private float _aRes = get_aRes(AFS_DEFAULT_VALUE);
        private float _gRes = get_gRes(GFS_DEFAULT_VALUE);

        private static float get_aRes(AccelFullScale ascale)
        {
            switch (ascale)
            {
                // Possible accelerometer scales (and their register bit settings) are:
                // 2 Gs (00), 4 Gs (01), 8 Gs (10), and 16 Gs  (11). 
                // Here's a bit of an algorith to calculate DPS/(ADC tick) based on that 2-bit value:
                case AccelFullScale.AFS_2G:
                    return 2.0f / 32768.0f;
                case AccelFullScale.AFS_4G:
                    return 4.0f / 32768.0f;
                case AccelFullScale.AFS_8G:
                    return 8.0f / 32768.0f;
                case AccelFullScale.AFS_16G:
                    return 16.0f / 32768.0f;
                default:
                    throw new Exception("get_aRes error");
            }
        }
        private static float get_gRes(GyroFullScale gscale)
        {
            switch (gscale)
            {
                // Possible gyro scales (and their register bit settings) are:
                // 250 DPS (00), 500 DPS (01), 1000 DPS (10), and 2000 DPS  (11). 
                case GyroFullScale.GFS_250DPS:
                    return 250.0f / 32768.0f;
                case GyroFullScale.GFS_500DPS:
                    return 500.0f / 32768.0f;
                case GyroFullScale.GFS_1000DPS:
                    return 1000.0f / 32768.0f;
                case GyroFullScale.GFS_2000DPS:
                    return 2000.0f / 32768.0f;
                default:
                    throw new Exception("get_aRes error");
            }
        }

        private void initiailze()
        {
            if (this.ID != 0x71)
            {
                throw new Exception("MPU9250 module initialize error");
            }

            this.reset();

            this.setSleepEnabled(false);
            var xOffset = this.getGyroXOffset();
            var yOffset = this.getGyroYOffset();
            var zOffset = this.getGyroZOffset();

            this.setSampleRate(0x00);

            this.setClockSource(ClockSource.CLOCK_PLL_ZGYRO);

            this.setDLPFMode(DLPFMode.DLPF_BW_188);

            this.setGyroFullScaleRange(GyroFullScale.GFS_2000DPS);

            this.setGyroXOffset(xOffset);
            this.setGyroXOffset(yOffset);
            this.setGyroXOffset(zOffset);

            this.setAccelFullScaleRange(AccelFullScale.AFS_8G);

        }


        // Function which accumulates gyro and accelerometer data after device initialization. It calculates the average
        // of the at-rest readings and then loads the resulting offsets into accelerometer and gyro bias registers.
        private void calibrate()
        {
            float[] dest1 = new float[3];
            float[] dest2 = new float[3];

            var data_2 = new byte[2];
            var data_12 = new byte[12];		// data array to hold accelerometer and gyro x, y, z, data

            var accel_temp = new ushort[3];
            var gyro_temp = new ushort[3];

            var gyro_bias = new int[3];
            var accel_bias = new int[3];

            // reset device
            this.WriteByte(Register.PWR_MGMT_1, 0x80);		// Write a one to bit 7 reset bit; toggle reset device
            Hardware.Library.Delay(100);

            // get stable time source; Auto select clock source to be PLL gyroscope reference if ready 
            // else use the internal oscillator, bits 2:0 = 001
            this.WriteByte(Register.PWR_MGMT_1, 0x01);
            this.WriteByte(Register.PWR_MGMT_2, 0x00);
            Hardware.Library.Delay(200);

            // Configure device for bias calculation
            this.WriteByte(Register.INT_ENABLE, 0x00);		// Disable all interrupts
            this.WriteByte(Register.FIFO_EN, 0x00);		// Disable FIFO
            this.WriteByte(Register.PWR_MGMT_1, 0x00);		// Turn on internal clock source
            this.WriteByte(Register.I2C_MST_CTRL, 0x00);		// Disable I2C master
            this.WriteByte(Register.USER_CTRL, 0x00);		// Disable FIFO and I2C master modes
            this.WriteByte(Register.USER_CTRL, 0x0C);		// Reset FIFO and DMP
            Hardware.Library.Delay(15);

            // Configure MPU6050 gyro and accelerometer for bias calculation
            this.WriteByte(Register.CONFIG, 0x01);		// Set low-pass filter to 188 Hz
            this.WriteByte(Register.SMPLRT_DIV, 0x00);		// Set sample rate to 1 kHz
            this.WriteByte(Register.GYRO_CONFIG, 0x00);		// Set gyro full-scale to 250 degrees per second, maximum sensitivity
            this.WriteByte(Register.ACCEL_CONFIG, 0x00);		// Set accelerometer full-scale to 2 g, maximum sensitivity

            ushort gyrosensitivity = 131;		// = 131 LSB/degrees/sec
            ushort accelsensitivity = 16384;		// = 16384 LSB/g

            // Configure FIFO to capture accelerometer and gyro data for bias calculation
            this.WriteByte(Register.USER_CTRL, 0x40);		// Enable FIFO  
            this.WriteByte(Register.FIFO_EN, 0x78);		// Enable gyro and accelerometer sensors for FIFO  (max size 512 bytes in MPU-9150)
            Hardware.Library.Delay(40);		// accumulate 40 samples in 40 milliseconds = 480 bytes

            // At end of sample accumulation, turn off FIFO sensor read
            this.WriteByte(Register.FIFO_EN, 0x00);		// Disable gyro and accelerometer sensors for FIFO

            this.Read(Register.FIFO_COUNTH, data_2);		// read FIFO sample count
            var fifo_count = ((ushort)data_2[0] << 8) | data_2[1];
            var packet_count = fifo_count / 12;// How many sets of full gyro and accelerometer data for averaging

            for (var i = 0; i < packet_count; i++)
            {
                this.Read(Register.FIFO_R_W, data_12);		// read data for averaging
                accel_temp[0] = (ushort)((data_12[0] << 8) | data_12[1]);		// Form signed 16-bit integer for each sample in FIFO
                accel_temp[1] = (ushort)((data_12[2] << 8) | data_12[3]);
                accel_temp[2] = (ushort)((data_12[4] << 8) | data_12[5]);
                gyro_temp[0] = (ushort)((data_12[6] << 8) | data_12[7]);
                gyro_temp[1] = (ushort)((data_12[8] << 8) | data_12[9]);
                gyro_temp[2] = (ushort)((data_12[10] << 8) | data_12[11]);

                accel_bias[0] += accel_temp[0];		// Sum individual signed 16-bit biases to get accumulated signed 32-bit biases
                accel_bias[1] += accel_temp[1];
                accel_bias[2] += accel_temp[2];
                gyro_bias[0] += gyro_temp[0];
                gyro_bias[1] += gyro_temp[1];
                gyro_bias[2] += gyro_temp[2];

            }
            accel_bias[0] /= packet_count;		// Normalize sums to get average count biases
            accel_bias[1] /= packet_count;
            accel_bias[2] /= packet_count;
            gyro_bias[0] /= packet_count;
            gyro_bias[1] /= packet_count;
            gyro_bias[2] /= packet_count;

            if (accel_bias[2] > 0)
            {
                accel_bias[2] -= accelsensitivity;
            }  // Remove gravity from the z-axis accelerometer bias calculation
            else
            {
                accel_bias[2] += accelsensitivity;
            }

            // Construct the accelerometer biases for push to the hardware accelerometer bias registers. These registers contain
            // factory trim values which must be added to the calculated accelerometer biases; on boot up these registers will hold
            // non-zero values. In addition, bit 0 of the lower byte must be preserved since it is used for temperature
            // compensation calculations. Accelerometer bias registers expect bias input as 2048 LSB per g, so that
            // the accelerometer biases calculated above must be divided by 8.

            var accel_bias_reg = new int[3];		// A place to hold the factory accelerometer trim biases
            this.Read(Register.XA_OFFSET_H, data_2);		// Read factory accelerometer trim values
            accel_bias_reg[0] = (data_2[0] << 8) | data_2[1];
            this.Read(Register.YA_OFFSET_H, data_2);
            accel_bias_reg[1] = (data_2[0] << 8) | data_2[1];
            this.Read(Register.ZA_OFFSET_H, data_2);
            accel_bias_reg[2] = (data_2[0] << 8) | data_2[1];

            uint mask = 1;		// Define mask for temperature compensation bit 0 of lower byte of accelerometer bias registers
            var mask_bit = new byte[3];		// Define array to hold mask bit for each accelerometer bias axis

            for (var i = 0; i < 3; i++)
            {
                if ((accel_bias_reg[i] & mask) != 0)
                {
                    mask_bit[i] = 0x01;		// If temperature compensation bit is set, record that fact in mask_bit
                }
            }

            // Construct total accelerometer bias, including calculated average accelerometer bias from above
            accel_bias_reg[0] -= (accel_bias[0] / 8);		// Subtract calculated averaged accelerometer bias scaled to 2048 LSB/g (16 g full scale)
            accel_bias_reg[1] -= (accel_bias[1] / 8);
            accel_bias_reg[2] -= (accel_bias[2] / 8);

            data_12[0] = (byte)((accel_bias_reg[0] >> 8) & 0xFF);
            data_12[1] = (byte)((accel_bias_reg[0]) & 0xFF);
            data_12[1] = (byte)(data_12[1] | mask_bit[0]);		// preserve temperature compensation bit when writing back to accelerometer bias registers
            data_12[2] = (byte)((accel_bias_reg[1] >> 8) & 0xFF);
            data_12[3] = (byte)((accel_bias_reg[1]) & 0xFF);
            data_12[3] = (byte)(data_12[3] | mask_bit[1]);		// preserve temperature compensation bit when writing back to accelerometer bias registers
            data_12[4] = (byte)((accel_bias_reg[2] >> 8) & 0xFF);
            data_12[5] = (byte)((accel_bias_reg[2]) & 0xFF);
            data_12[5] = (byte)(data_12[5] | mask_bit[2]);		// preserve temperature compensation bit when writing back to accelerometer bias registers

            // Apparently this is not working for the acceleration biases in the MPU-9250
            // Are we handling the temperature correction bit properly?
            // Push accelerometer biases to hardware registers
            // writeByte(MPU9250_ADDRESS, XA_OFFSET_H, data[0]);
            // writeByte(MPU9250_ADDRESS, XA_OFFSET_L, data[1]);
            // writeByte(MPU9250_ADDRESS, YA_OFFSET_H, data[2]);
            // writeByte(MPU9250_ADDRESS, YA_OFFSET_L, data[3]);
            // writeByte(MPU9250_ADDRESS, ZA_OFFSET_H, data[4]);
            // writeByte(MPU9250_ADDRESS, ZA_OFFSET_L, data[5]);

            // Output scaled accelerometer biases for display in the main program
            this.AccBias = new Core.Data.Vector3(
                (float)accel_bias[0] / (float)accelsensitivity,
                (float)accel_bias[1] / (float)accelsensitivity,
                (float)accel_bias[2] / (float)accelsensitivity
            );



            // Construct the gyro biases for push to the hardware gyro bias registers, which are reset to zero upon device startup
            data_12[0] = (byte)((-gyro_bias[0] / 4 >> 8) & 0xFF);		// Divide by 4 to get 32.9 LSB per deg/s to conform to expected bias input format
            data_12[1] = (byte)((-gyro_bias[0] / 4) & 0xFF);		// Biases are additive, so change sign on calculated average gyro biases
            data_12[2] = (byte)((-gyro_bias[1] / 4 >> 8) & 0xFF);
            data_12[3] = (byte)((-gyro_bias[1] / 4) & 0xFF);
            data_12[4] = (byte)((-gyro_bias[2] / 4 >> 8) & 0xFF);
            data_12[5] = (byte)((-gyro_bias[2] / 4) & 0xFF);

            // Push gyro biases to hardware registers
            this.WriteByte(Register.XG_OFFSET_H, data_12[0]);
            this.WriteByte(Register.XG_OFFSET_L, data_12[1]);
            this.WriteByte(Register.YG_OFFSET_H, data_12[2]);
            this.WriteByte(Register.YG_OFFSET_L, data_12[3]);
            this.WriteByte(Register.ZG_OFFSET_H, data_12[4]);
            this.WriteByte(Register.ZG_OFFSET_L, data_12[5]);

            // Output scaled gyro biases for display in the main program
            this.GyroBias = new Core.Data.Vector3(
                (float)gyro_bias[0] / (float)gyrosensitivity,
                (float)gyro_bias[1] / (float)gyrosensitivity,
                (float)gyro_bias[2] / (float)gyrosensitivity
            );

            Hardware.Library.Delay(200);
        }

        private void reset()
        {
            //  * CLK_SEL | Clock Source
            //  * --------+--------------------------------------
            //  * 0       | Internal oscillator
            //  * 1       | PLL with X Gyro reference
            //  * 2       | PLL with Y Gyro reference
            //  * 3       | PLL with Z Gyro reference
            //  * 4       | PLL with external 32.768kHz reference
            //  * 5       | PLL with external 19.2MHz reference
            //  * 6       | Reserved
            //  * 7       | Stops the clock and keeps the timing generator in reset

            // bit 7 len 1
            this.WriteBit(Register.PWR_MGMT_1, 0x07, true);
            Hardware.Library.Delay(120);    // Wait for all registers to reset
        }

        private void setSleepEnabled(bool enable)
        {
            // bit 6 len 1
            this.WriteBit(Register.PWR_MGMT_1, 6, enable);
            Hardware.Library.Delay(10);

            if (!enable)
            {
                this.WriteByte(Register.PWR_MGMT_1, 0x00);  // Clear sleep mode bit (6), enable all sensors
                Hardware.Library.Delay(10);
            }
        }

        private void setSampleRate(byte sampleRate)
        {
            // bit 7 len 8
            this.WriteByte(Register.SMPLRT_DIV, sampleRate);
            Hardware.Library.Delay(10);
        }

        private void setClockSource(ClockSource clockSource)
        {
            // * CLK_SEL | Clock Source
            // * --------+--------------------------------------
            // * 0       | Internal oscillator
            // * 1       | PLL with X Gyro reference
            // * 2       | PLL with Y Gyro reference
            // * 3       | PLL with Z Gyro reference
            // * 4       | PLL with external 32.768kHz reference
            // * 5       | PLL with external 19.2MHz reference
            // * 6       | Reserved
            // * 7       | Stops the clock and keeps the timing generator in reset

            // bit 2 len 3
            this.WriteBits(Register.PWR_MGMT_1, 2, 3, Convert.ToByte(clockSource));
            Hardware.Library.Delay(10);
        }

        private void setDLPFMode(DLPFMode mode)
        {
            //  *          |   ACCELEROMETER    |           GYROSCOPE
            //  * DLPF_CFG | Bandwidth | Delay  | Bandwidth | Delay  | Sample Rate
            //  * ---------+-----------+--------+-----------+--------+-------------
            //  * 0        | 260Hz     | 0ms    | 256Hz     | 0.98ms | 8kHz
            //  * 1        | 184Hz     | 2.0ms  | 188Hz     | 1.9ms  | 1kHz
            //  * 2        | 94Hz      | 3.0ms  | 98Hz      | 2.8ms  | 1kHz
            //  * 3        | 44Hz      | 4.9ms  | 42Hz      | 4.8ms  | 1kHz
            //  * 4        | 21Hz      | 8.5ms  | 20Hz      | 8.3ms  | 1kHz
            //  * 5        | 10Hz      | 13.8ms | 10Hz      | 13.4ms | 1kHz
            //  * 6        | 5Hz       | 19.0ms | 5Hz       | 18.6ms | 1kHz
            //  * 7        |   -- Reserved --   |   -- Reserved --   | Reserved

            // bit 2 len 3
            this.WriteBits(Register.CONFIG, 2, 3, Convert.ToByte(mode));
            Hardware.Library.Delay(10);
        }

        private void setAccelFullScaleRange(AccelFullScale afs)
        {
            //  * 0 = +/- 2g
            //  * 1 = +/- 4g
            //  * 2 = +/- 8g
            //  * 3 = +/- 16g

            // bit 4 len 2
            this.WriteBits(Register.ACCEL_CONFIG, 4, 2, Convert.ToByte(afs));
            Hardware.Library.Delay(10);
        }

        private void setGyroFullScaleRange(GyroFullScale gfs)
        {
            //  * 0 = +/- 250 degrees/sec
            //  * 1 = +/- 500 degrees/sec
            //  * 2 = +/- 1000 degrees/sec
            //  * 3 = +/- 2000 degrees/sec

            // bit 4 len 2
            this.WriteBits(Register.GYRO_CONFIG, 4, 2, Convert.ToByte(gfs));
            Hardware.Library.Delay(10);
        }

        private byte getGyroXOffset()
        {
            //bit 6 len 6
            return this.ReadBits(Register.XG_OFFS_TC, 6, 6);
        }
        private void setGyroXOffset(byte offset)
        {
            //bit 6 len 6
            this.WriteBits(Register.XG_OFFS_TC, 6, 6, offset);
        }

        private byte getGyroYOffset()
        {
            //bit 6 len 6
            return this.ReadBits(Register.YG_OFFS_TC, 6, 6);
        }
        private void setGyroYOffset(byte offset)
        {
            //bit 6 len 6
            this.WriteBits(Register.YG_OFFS_TC, 6, 6, offset);
        }

        private byte getGyroZOffset()
        {
            //bit 6 len 6
            return this.ReadBits(Register.ZG_OFFS_TC, 6, 6);
        }
        private void setGyroZOffset(byte offset)
        {
            //bit 6 len 6
            this.WriteBits(Register.ZG_OFFS_TC, 6, 6, offset);
        }


        byte[] _accel_buf = new byte[6];

        private Core.Data.Vector3 readAccelData()
        {
            this.Read(Register.ACCEL_XOUT_H, _accel_buf);
            return new Core.Data.Vector3(
                (short)(_accel_buf[0] << 8 | _accel_buf[1]),
                (short)(_accel_buf[2] << 8 | _accel_buf[3]),
                (short)(_accel_buf[4] << 8 | _accel_buf[5])
            );
        }

        byte[] _gyro_buf = new byte[6];
        private Core.Data.Vector3 readGyroData()
        {
            this.Read(Register.GYRO_XOUT_H, _gyro_buf);
            return new Core.Data.Vector3(
                (short)(_gyro_buf[0] << 8 | _gyro_buf[1]),
                (short)(_gyro_buf[2] << 8 | _gyro_buf[3]),
                (short)(_gyro_buf[4] << 8 | _gyro_buf[5])
            );
        }

    }
}
