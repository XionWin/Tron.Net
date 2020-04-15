using System;

namespace Tron.Device.Gyro.MPU9250
{
    public enum DLPFMode : byte
    {
        DLPF_BW_256 = 0x00,
        DLPF_BW_188 = 0x01,
        DLPF_BW_98 = 0x02,
        DLPF_BW_42 = 0x03,
        DLPF_BW_20 = 0x04,
        DLPF_BW_10 = 0x05,
        DLPF_BW_5 = 0x06,
    }
}
