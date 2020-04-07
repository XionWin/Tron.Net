using System;

namespace Tron.Device.Indicator
{
    internal class BuzzerIndicator
    {
        private const Hardware.GPIOPins BUZZER_PIN = Hardware.GPIOPins.GPIO_22;
        private BuzzerIndicator()
        {
            Hardware.GPIO.CreatePin(BUZZER_PIN).State = Hardware.PinState.High;
        }


        private static BuzzerIndicator _Instance = null;
        public static BuzzerIndicator Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new BuzzerIndicator();
                }
                return _Instance;
            }
        }

        private bool _enable = false;
        public bool Enable
        {
            get => this._enable;
            set
            {
                if (this._enable != value)
                {
#if ENABLE_BUZZER
                    Hardware.GPIO.CreatePin(BUZZER_PIN).State = value ? Hardware.PinState.Low : Hardware.PinState.High;
#endif
                    this._enable = value;
                }
            }
        }

        public void Reset()
        {
            this.Enable = false;
            Hardware.GPIO.CreatePin(BUZZER_PIN).State = Hardware.PinState.High;
        }
    }
}
