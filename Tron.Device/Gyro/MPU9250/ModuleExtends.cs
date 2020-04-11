using System;

namespace Tron.Device.Gyro.MPU9250
{
    public partial class Module : Hardware.I2CDevice<Register>, IGyro
    {
        private Ascale _ascale = Ascale.AFS_2G;
        private Gscale _gscale = Gscale.GFS_1000DPS;
        private byte _sampleRate = 0x04;

        private void Initiailze()
        {
            if (this.ID != 0x71)
            {
                throw new Exception("MPU9250 module initialize error");
            }

            // wake up device
            this.WriteByte(Register.PWR_MGMT_1, 0x00);		// Clear sleep mode bit (6), enable all sensors 
            Hardware.Library.Delay(100);

            // get stable time source
            this.WriteByte(Register.PWR_MGMT_1, 0x01);		// Auto select clock source to be PLL gyroscope reference if ready else
            Hardware.Library.Delay(200);

            // Configure Gyro and Thermometer
            // Disable FSYNC and set thermometer and gyro bandwidth to 41 and 42 Hz, respectively; 
            // minimum delay time for this setting is 5.9 ms, which means sensor fusion update rates cannot
            // be higher than 1 / 0.0059 = 170 Hz
            // DLPF_CFG = bits 2:0 = 011; this limits the sample rate to 1000 Hz for both
            // With the MPU9250, it is possible to get gyro sample rates of 32 kHz (!), 8 kHz, or 1 kHz
            this.WriteByte(Register.CONFIG, 0x03);

            // Set sample rate = gyroscope output rate/(1 + SMPLRT_DIV)
            this.WriteByte(Register.SMPLRT_DIV, this._sampleRate);        // Use a 200 Hz rate; a rate consistent with the filter update rate 
                                                                    // determined inset in CONFIG above

            // Set gyroscope full scale range
            // Range selects FS_SEL and AFS_SEL are 0 - 3, so 2-bit values are left-shifted into positions 4:3
            byte c = this.ReadByte(Register.GYRO_CONFIG);		// get current GYRO_CONFIG register value

            // c = c & ~0xE0;		// Clear self-test bits [7:5] 
            c = (byte)(c & ~0x02);		// Clear Fchoice bits [1:0] 
            c = (byte)(c & ~0x18);		// Clear AFS bits [4:3]
            c = (byte)(c | (byte)this._gscale << 3);       // Set full scale range for the gyro
                                                // c =| 0x00;		// Set Fchoice for the gyro to 11 by writing its inverse to bits 1:0 of GYRO_CONFIG
            this.WriteByte(Register.GYRO_CONFIG, c);		// Write new GYRO_CONFIG value to register

            // Set accelerometer full-scale range configuration
            c = this.ReadByte(Register.ACCEL_CONFIG);		// get current ACCEL_CONFIG register value

            // c = c & ~0xE0;		// Clear self-test bits [7:5] 
            c = (byte)(c & ~0x18);		// Clear AFS bits [4:3]
            c = (byte)(c | (byte)this._ascale << 3);		// Set full scale range for the accelerometer 
            this.WriteByte(Register.ACCEL_CONFIG, c);		// Write new ACCEL_CONFIG register value

            // Set accelerometer sample rate configuration
            // It is possible to get a 4 kHz sample rate from the accelerometer by choosing 1 for
            // accel_fchoice_b bit [3]; in this case the bandwidth is 1.13 kHz
            c = this.ReadByte(Register.ACCEL_CONFIG_2);		// get current ACCEL_CONFIG2 register value
            c = (byte)(c & ~0x0F);		// Clear accel_fchoice_b (bit 3) and A_DLPFG (bits [2:0])  
            c = (byte)(c | 0x03);		//Set accelerometer rate to 1 kHz and bandwidth to 41 Hz
            this.WriteByte(Register.ACCEL_CONFIG_2, c);		// Write new ACCEL_CONFIG2 register value

            // The accelerometer, gyro, and thermometer are set to 1 kHz sample rates, 
            // but all these rates are further reduced by a factor of 5 to 200 Hz because of the SMPLRT_DIV setting

            // Configure Interrupts and Bypass Enable
            // Set interrupt pin active high, push-pull, hold interrupt pin level HIGH until interrupt cleared,
            // clear on read of INT_STATUS, and enable I2C_BYPASS_EN so additional chips 
            // can join the I2C bus and all can be controlled by the Arduino as master
            this.WriteByte(Register.INT_PIN_CFG, 0x10);		// INT is 50 microsecond pulse and any read to clear  
            this.WriteByte(Register.INT_ENABLE, 0x01);		// Enable data ready (bit 0) interrupt
            Hardware.Library.Delay(100);

            this.WriteByte(Register.USER_CTRL, 0x20);		// Enable I2C Master mode  
            this.WriteByte(Register.I2C_MST_CTRL, 0x1D);		// I2C configuration STOP after each transaction, master I2C bus at 400 KHz
            this.WriteByte(Register.I2C_MST_DELAY_CTRL, 0x81);		// Use blocking data retreival and enable delay for mag sample rate mismatch
            this.WriteByte(Register.I2C_SLV4_CTRL, 0x01);		// Delay mag data retrieval to once every other accel/gyro data sample
            Hardware.Library.Delay(100);
        }


        // Function which accumulates gyro and accelerometer data after device initialization. It calculates the average
        // of the at-rest readings and then loads the resulting offsets into accelerometer and gyro bias registers.
        private (float ax, float ay, float az, float gx, float gy, float gz) calibrate()
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
            dest1[0] = (float)accel_bias[0] / (float)accelsensitivity;
            dest1[1] = (float)accel_bias[1] / (float)accelsensitivity;
            dest1[2] = (float)accel_bias[2] / (float)accelsensitivity;



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
            dest2[0] = (float)gyro_bias[0] / (float)gyrosensitivity;
            dest2[1] = (float)gyro_bias[1] / (float)gyrosensitivity;
            dest2[2] = (float)gyro_bias[2] / (float)gyrosensitivity;

            Hardware.Library.Delay(100);
            this.Initiailze();
            return (dest1[0], dest1[1], dest1[2], dest2[0], dest2[1], dest2[2]);
        }

        private void Reset()
        {
            // reset device
            this.WriteByte(Register.PWR_MGMT_1, 0x80); // Set bit 7 to reset MPU9250
            Hardware.Library.Delay(100); // Wait for all registers to reset 
        }

        byte[] _accel_buf = new byte[6];

        private Core.Data.Vector3 ReadAccelData()
        {
            this.Read(Register.ACCEL_XOUT_H, _accel_buf);
            return new Core.Data.Vector3(
                (_accel_buf[0] << 8) | _accel_buf[1],
                (_accel_buf[2] << 8) | _accel_buf[3],
                (_accel_buf[4] << 8) | _accel_buf[5]
            );
        }

        byte[] _gyro_buf = new byte[6];
        private Core.Data.Vector3 ReadGyroData()
        {
            this.Read(Register.GYRO_XOUT_H, _gyro_buf);
            return new Core.Data.Vector3(
                (_gyro_buf[0] << 8) | _gyro_buf[1],
                (_gyro_buf[2] << 8) | _gyro_buf[3],
                (_gyro_buf[4] << 8) | _gyro_buf[5]
            );
        }

    }
}
