using System;

namespace Tron.Device.Gyro.MPU9250
{
    public enum Register : byte
    {
        MODULE_ADDRESS = 0x68,


        USER_CTRL = 0x6A,  // Bit 7 enable DMP, bit 3 reset DMP
        PWR_MGMT_1 = 0x6B,
        PWR_MGMT_2 = 0x6C,

        FIFO_EN = 0x23,
        FIFO_COUNTH = 0x72,
        FIFO_COUNTL = 0x73,
        FIFO_R_W = 0x74,

        I2C_MST_CTRL = 0x24,
        I2C_MST_DELAY_CTRL = 0x67,
        I2C_SLV4_CTRL = 0x34,

        WHO_AM_I = 0x75,

        SMPLRT_DIV = 0x19,
        CONFIG = 0x1A,
        ACCEL_CONFIG = 0x1C,
        ACCEL_CONFIG_2 = 0x1D,
        GYRO_CONFIG = 0x1B,

        INT_PIN_CFG = 0x37,
        INT_ENABLE = 0x38,

        ACCEL_ADDRESS = 0x3B,
        ACCEL_X_H = 0x3B,
        ACCEL_X_L = 0x3C,
        ACCEL_Y_H = 0x3D,
        ACCEL_Y_L = 0x3E,
        ACCEL_Z_H = 0x3F,
        ACCEL_Z_L = 0x40,

        GYRO_ADDRESS = 0x43,
        GYRO_X_H = 0x43,
        GYRO_X_L = 0x44,
        GYRO_Y_H = 0x45,
        GYRO_Y_L = 0x46,
        GYRO_Z_H = 0x47,
        GYRO_Z_L = 0x48,


        MAG_ADDRESS = 0x18,
        MAG_X_L = 0x03,
        MAG_X_H = 0x04,
        MAG_Y_L = 0x05,
        MAG_Y_H = 0x06,
        MAG_Z_L = 0x07,
        MAG_Z_H = 0x08,

        XA_OFFSET_H = 0x77,
        XA_OFFSET_L = 0x78,
        YA_OFFSET_H = 0x7A,
        YA_OFFSET_L = 0x7B,
        ZA_OFFSET_H = 0x7D,
        ZA_OFFSET_L = 0x7E,

        XG_OFFSET_H = 0x13,  // User-defined trim values for gyroscope
        XG_OFFSET_L = 0x14,
        YG_OFFSET_H = 0x15,
        YG_OFFSET_L = 0x16,
        ZG_OFFSET_H = 0x17,
        ZG_OFFSET_L = 0x18,

    }
}
