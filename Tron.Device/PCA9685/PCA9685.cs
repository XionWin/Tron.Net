using System;
using System.Collections.Generic;

namespace Tron.Device
{
    public class PCA9685 : BusDevice<Hardware.II2C>
    {
        private const Hardware.GPIOPins PIN_EN = Hardware.GPIOPins.GPIO_04;
        private const byte ADDRESS = 0x40;
        private const byte MODE1 = 0x00;
        private const byte MODE2 = 0x01;
        private const int PRE_SCALE = 0xFE;
        private const int CLOCK_FREQ = 25000000;     //25MHz default osc clock

        private const int PWM_SCALE = 4095;
        private const int PWM_FREQUENCY = 400;
        private const short PWM_LOW_STATIC = 960 * 2;
        private const short PWM_LOW_DYNAMIC = 970 * 2;   //1938 base on 400 frequency
        private const short PWM_HIGH = 1965 * 2;     //3930 base on 400 frequency


        private byte _sleep_data = 0x00;
        public PCA9685(Hardware.II2C i2c, IEnumerable<Channel> channels) : base(i2c)
        {
            this.Channels = channels;
            Init();
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
            this.BUS.SlaveAddress = ADDRESS;
            this.SetFreq(PWM_FREQUENCY);
            this.Reset();
            Hardware.Library.Delay(1000);
        }

        private void SetFreq(int freq)
        {
            byte prescale = (byte)((CLOCK_FREQ / PWM_SCALE / freq) - 1);

            this.Sleep();
            this.BUS.WriteByte(PRE_SCALE, prescale);
            this.Wake();
        }

        public void SetValue(Channel channel, short value)
        {
            if (value <= 1000)
            {
                var v = value == 0 ? PWM_LOW_STATIC :
                (short)(PWM_LOW_DYNAMIC + ((PWM_HIGH - PWM_LOW_DYNAMIC) / 1000 * (value - 1)));
                this.Set(channel, 0, v);
            }
        }

        private void Set(Channel channel, short on, short off)
        {
            this.BUS.SlaveAddress = ADDRESS;
            var channel_base_reg = (byte)channel;
            
            this.BUS.WriteByte((byte)(channel_base_reg + 2), (byte)off);
            this.BUS.WriteByte((byte)(channel_base_reg + 3), (byte)(off >> 8));
        }
        private void Reset()
        {
            foreach (var channel in this.Channels)
            {
                this.Set(channel, 0, PWM_LOW_STATIC);
            }
        }
        private void Sleep()
        {
            this.Reset();
            this._sleep_data = this.BUS.ReadByte(MODE1);
            var new_mode = (byte)((this._sleep_data & 0x7F) | 0x10); // sleep
            this.BUS.WriteByte(MODE1, new_mode);
        }

        public void Wake()
        {
            var old_mode = this._sleep_data;
            this.BUS.WriteByte(MODE1, old_mode);
            Hardware.Library.DelayMicroseconds(500);
            this.BUS.WriteByte(MODE1, (byte)(old_mode & 0x6F));
            this.Reset();
            Hardware.Library.DelayMicroseconds(500);
        }


    }
}
