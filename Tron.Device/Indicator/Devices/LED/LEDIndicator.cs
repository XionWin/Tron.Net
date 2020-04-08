using System;

namespace Tron.Device.Indicator
{
    internal class LEDIndicator
    {
        private const Hardware.GPIOPins RED_PIN = Hardware.GPIOPins.GPIO_19;
        private const Hardware.GPIOPins GREEN_PIN = Hardware.GPIOPins.GPIO_20;
        private const Hardware.GPIOPins BLUE_PIN = Hardware.GPIOPins.GPIO_21;
        private LEDIndicator()
        {
            Hardware.GPIO.CreatePin(RED_PIN).State = Hardware.PinState.High;
            Hardware.GPIO.CreatePin(GREEN_PIN).State = Hardware.PinState.High;
            Hardware.GPIO.CreatePin(BLUE_PIN).State = Hardware.PinState.High;
        }

        private static LEDIndicator _Instance = null;
        public static LEDIndicator Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LEDIndicator();
                }
                return _Instance;
            }
        }

        private bool _enable;
        public bool Enable
        {
            get => this._enable;
            set
            {
                if (this._enable != value)
                {
                    this._enable = value;
                    if(!this._enable)
                    {
                        this.Reset();
                    }
                }
            }
        }

        private IndicatorColor _color = IndicatorColor.NULL;
        public IndicatorColor Color
        {
            get => this._color;
            set
            {
                if (this.Enable && this._color != value)
                {
                    Hardware.GPIO.CreatePin(RED_PIN).State = ((byte)value & 0x01) == 0x01 ? Hardware.PinState.Low : Hardware.PinState.High;
                    Hardware.GPIO.CreatePin(GREEN_PIN).State = ((byte)value & 0x02) == 0x02 ? Hardware.PinState.Low : Hardware.PinState.High;
                    Hardware.GPIO.CreatePin(BLUE_PIN).State = ((byte)value & 0x04) == 0x04 ? Hardware.PinState.Low : Hardware.PinState.High;
                    this._color = value;
                }
            }
        }

        public void Reset()
        {
            this.Color = IndicatorColor.NULL;
            Hardware.GPIO.CreatePin(RED_PIN).State = Hardware.PinState.High;
            Hardware.GPIO.CreatePin(GREEN_PIN).State = Hardware.PinState.High;
            Hardware.GPIO.CreatePin(BLUE_PIN).State = Hardware.PinState.High;
        }
    }
}
