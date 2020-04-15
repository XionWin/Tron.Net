using System;

namespace Tron.Device.Gyro.MPU9250
{
    public enum Register : byte
    {
        MODULE_ADDRESS = 0x68,
        WHO_AM_I = 0x75,

        EXT_SENS_DATA_00 = 0x49,
        AK8963_MODULE_ADDRESS = AK8963.Register.MODULE_ADDRESS,
        AK8963_WHO_AM_I = AK8963.Register.WHO_AM_I,
        AK8963_CNTL = AK8963.Register.CNTL,
        AK8963_CNTL2 = AK8963.Register.CNTL2,

        AK8963_ASAX = AK8963.Register.ASAX,
        AK8963_ASAY = AK8963.Register.ASAY,
        AK8963_ASAZ = AK8963.Register.ASAZ,

        AK8963_XOUT_L = AK8963.Register.XOUT_L,
        AK8963_XOUT_H = AK8963.Register.XOUT_H,
        AK8963_YOUT_L = AK8963.Register.YOUT_L,
        AK8963_YOUT_H = AK8963.Register.YOUT_H,
        AK8963_ZOUT_L = AK8963.Register.ZOUT_L,
        AK8963_ZOUT_H = AK8963.Register.ZOUT_H,



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


        SMPLRT_DIV = 0x19,
        CONFIG = 0x1A,
        ACCEL_CONFIG = 0x1C,
        ACCEL_CONFIG_2 = 0x1D,
        GYRO_CONFIG = 0x1B,

        INT_PIN_CFG = 0x37,
        INT_ENABLE = 0x38,


        ACCEL_XOUT_H = 0x3B,
        ACCEL_XOUT_L = 0x3C,
        ACCEL_YOUT_H = 0x3D,
        ACCEL_YOUT_L = 0x3E,
        ACCEL_ZOUT_H = 0x3F,
        ACCEL_ZOUT_L = 0x40,

        GYRO_XOUT_H = 0x43,
        GYRO_XOUT_L = 0x44,
        GYRO_YOUT_H = 0x45,
        GYRO_YOUT_L = 0x46,
        GYRO_ZOUT_H = 0x47,
        GYRO_ZOUT_L = 0x48,


        MAG_XOUT_L = 0x03,
        MAG_XOUT_H = 0x04,
        MAG_YOUT_L = 0x05,
        MAG_YOUT_H = 0x06,
        MAG_ZOUT_L = 0x07,
        MAG_ZOUT_H = 0x08,

        XG_OFFS_TC = 0x00,
        YG_OFFS_TC = 0x01,
        ZG_OFFS_TC = 0x02,

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


        I2C_SLV0_ADDR = 0x25,
        I2C_SLV0_REG = 0x26,
        I2C_SLV0_CTRL = 0x27,
        I2C_SLV0_DO = 0x63,

    }
}
