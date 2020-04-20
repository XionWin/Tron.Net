using System;
using System.Collections.Generic;

namespace Tron.Device.MotorControl.PCA9685
{
    public class Module : Hardware.I2CDevice<Register>
    {
        private const Hardware.GPIOPins PIN_EN = Hardware.GPIOPins.GPIO_18;
        private const int CLOCK_FREQ = 25000000;     //25MHz default osc clock

        private const int PWM_SCALE = 4095;
        private const int PWM_FREQUENCY = 400;
        private const short PWM_STATIC_LOW = 960 * 2;
        private const short PWM_DYNAMIC_LOW = 970 * 2;   //1938 base on 400 frequency
        private const short PWM_HIGH = 1965 * 2;     //3930 base on 400 frequency


        private byte _sleep_data = 0x00;
        public Module(IEnumerable<Channel> channels)
            : base(new Hardware.I2C((byte)Register.MODULE_ADDRESS, Hardware.I2CClockDivider.CLOCK_DIVIDER_200))
        {
            this.Channels = channels;
            Init();
        }
        public override byte ID
        {
            get => 0x00;
        }

        public IEnumerable<Channel> Channels
        {
            get;
            private set;
        }

        public bool Enable
        {
            set
            {
                Hardware.GPIO.CreatePin(PIN_EN).State = value ? Hardware.PinState.Low : Hardware.PinState.High;
            }
        }

        private void Init()
        {
            this.SetFreq(PWM_FREQUENCY);
            this.Reset();
            Hardware.Library.Delay(1000);
        }

        private void SetFreq(int freq)
        {
            byte prescale = (byte)((CLOCK_FREQ / PWM_SCALE / freq) - 1);

            this.Sleep();
            this.WriteByte(Register.PRE_SCALE, prescale);
            this.Wake();
        }

        public void SetValue(Channel channel, short value)
        {
            if (value >= 0 && value <= 800)
            {
                var v = value == 0 ? PWM_STATIC_LOW :
                (short)(PWM_DYNAMIC_LOW + ((PWM_HIGH - PWM_DYNAMIC_LOW) / 1000 * (value - 1)));
                this.Set(channel, 0, v);
            }
        }

        private void Set(Channel channel, short on, short off)
        {
            var channel_base_reg = (byte)channel;

            this.WriteByte((byte)(channel_base_reg + 2), (byte)off);
            this.WriteByte((byte)(channel_base_reg + 3), (byte)(off >> 8));
        }
        private void Reset()
        {
            foreach (var channel in this.Channels)
            {
                this.Set(channel, 0, PWM_STATIC_LOW);
            }
        }
        private void Sleep()
        {
            this.Reset();
            this._sleep_data = this.ReadByte(Register.MODE1);
            var new_mode = (byte)((this._sleep_data & 0x7F) | 0x10); // sleep
            this.WriteByte(Register.MODE1, new_mode);
        }

        public void Wake()
        {
            var old_mode = this._sleep_data;
            this.WriteByte(Register.MODE1, old_mode);
            Hardware.Library.DelayMicroseconds(500);
            this.WriteByte(Register.MODE1, (byte)(old_mode & 0x6F));
            this.Reset();
            Hardware.Library.DelayMicroseconds(500);
        }


    }
}
