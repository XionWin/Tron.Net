namespace Tron.Device
{
    public enum Channel: byte
    {
        Channel0 = 0x06,
        Channel1 = 0x0A,
        Channel2 = 0x0E,
        Channel3 = 0x12,
        Channel4 = 0x16,
        Channel5 = 0x1A,
        Channel6 = 0x1E,
        Channel7 = 0x22,
        Channel8 = 0x26,
        Channel9 = 0x2A,
        ChannelA = 0x2E,
        ChannelB = 0x32,
        ChannelC = 0x36,
        ChannelD = 0x3A,
        ChannelE = 0x3E,
        ChannelF = 0x42,

        // Don't use this register, it will make some delay.
        // ChannelALL = 0xFA
    }
}