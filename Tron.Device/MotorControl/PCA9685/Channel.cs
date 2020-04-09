namespace Tron.Device.MotorControl.PCA9685
{
    public enum Channel: byte
    {
        // Don't use this register, it will make some delay.
        // ChannelALL = 0xFA

        Channel0 = Register.Channel0,
        Channel1 = Register.Channel1,
        Channel2 = Register.Channel2,
        Channel3 = Register.Channel3,
        Channel4 = Register.Channel4,
        Channel5 = Register.Channel5,
        Channel6 = Register.Channel6,
        Channel7 = Register.Channel7,
        Channel8 = Register.Channel8,
        Channel9 = Register.Channel9,
        ChannelA = Register.ChannelA,
        ChannelB = Register.ChannelB,
        ChannelC = Register.ChannelC,
        ChannelD = Register.ChannelD,
        ChannelE = Register.ChannelE,
        ChannelF = Register.ChannelF,
    }
}