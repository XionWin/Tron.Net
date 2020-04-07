using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
	public interface IPWM
	{
		#region Properties
		/// <summary>
		/// The range of PWM
		/// </summary>
		PWMClockDivider ClockDivider { get; set; }
		int Range { get; set; }
		int Value { get; set; }
		#endregion

		#region Functions
		void Init();
		#endregion
	}

	public partial class PWM : IPWM
	{
		#region Fields
        private  int _channel = 0;
        private PWMMode _mode = PWMMode.MarkSpace;

		private PWMClockDivider _clockDivider = PWMClockDivider.CLOCK_DIVIDER_2048;
		private int _range = 0;

		private int _value = 0;
        #endregion

        #region Properties
		public PWMClockDivider ClockDivider
		{
			get => this._clockDivider;
			set
			{
				if (this._clockDivider != value)
				{
                    BCM2835_PWM.SetClockDivider(value);
				}
			}
		}
		public int Range
		{
			get => this._range;
			set
			{
				if (this._range != value)
				{
                    BCM2835_PWM.SetRange(this._channel, value);
				}
			}
		}
		public int Value
		{
			get => this._value;
			set
			{
				if (this._value != value)
				{
                    BCM2835_PWM.SetData(this._channel, value);
				}
			}
		}
        #endregion

		
		private static IPWM _Instance = null;
		public static IPWM Instance
		{
			get
			{
				if(_Instance == null)
				{
					_Instance = new PWM();
					_Instance.Init();
				}
				return _Instance;
			}
		}



	}

	partial class PWM
	{
		protected PWM()
		{
		}

        public void Init()
        {
			BCM2835_GPIO.DirectPin(GPIOPins.GPIO_18, GPIODirection.ALT5);
            BCM2835_PWM.SetClockDivider(this.ClockDivider);
            BCM2835_PWM.SetMode(this._channel, this._mode, true);
        }
		
		#region Implement IPWM Functions
        
		#endregion

		
		

	}
}
