using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
	public partial class GPIO : IGPIO
	{
		#region Static Fields
		private static ConcurrentDictionary<GPIOPins, GPIO> _ExportedPins = new ConcurrentDictionary<GPIOPins, GPIO>();
		#endregion

		#region Fields
		private readonly GPIOPins _pin = GPIOPins.GPIO_NONE;
		private GPIODirection _direction = GPIODirection.In;
		private GPIOResistor _resistor = GPIOResistor.OFF;
		private PinState _state = PinState.Low;
		#endregion

		#region Properties
		/// <summary>
		/// Dictionary that stores created (exported) pins that where not disposed.
		/// </summary>
		public static IEnumerable<IGPIO> ExportedPins
		{
			get => _ExportedPins.Values;
		}
		#endregion

		#region Implement IGPIO
		public GPIOPins Pin
		{
			get => this._pin;
		}

		public GPIOPinMask Mask
		{
			get => (GPIOPinMask)(1 << (byte)this.Pin); //Pin-Value has a low range (0-~32), so even casting to byte would be ok.
		}

		#endregion

	}

	partial class GPIO
	{

		/// <summary>
		/// Access to the specified GPIO setup with the specified direction with the specified initial value
		/// </summary>
		/// <param name="pin">The GPIO pin</param>
		/// <param name="direction">Direction</param>
		/// <param name="initialValue">Initial Value</param>
		private GPIO(GPIOPins pin, GPIODirection direction, PinState pinState)
		{
			if (pin == GPIOPins.GPIO_NONE)
			{
				throw new ArgumentException("Invalid pin");
			}
		
			if (_ExportedPins.ContainsKey(pin))
			{
				throw new Exception("Cannot use pin with multiple instances. Unexport the previous instance with Dispose() first! (pin " + (byte)pin + ")");
			}
			_ExportedPins[pin] = this;
			_pin = pin;
			try
			{
				this.Direction = direction;
				this.State = pinState;
				Write(pinState);
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
		
		public static IGPIO CreatePin(GPIOPins pin, GPIODirection dir = GPIODirection.Out, PinState pinState = PinState.Low)
		{
			if (_ExportedPins.ContainsKey(pin))
			{
				if (_ExportedPins[pin].Direction != dir)
					_ExportedPins[pin].Direction = dir;
				if (_ExportedPins[pin].State != pinState)
					_ExportedPins[pin].State = pinState;
				return _ExportedPins[pin];
			}
			try
			{
				return new GPIO(pin, dir, pinState);
			}
#if DEBUG
			catch (Exception e)
			{
				throw new Exception("Unable to create pin " + (uint)pin + ". Infomation: " + e.ToString());
			}
#else
			catch //stuff like lib load problems, wrong exports, etc...
			{
				return null;
			}
#endif
		}
		
		/// <summary>
		/// Gets or sets the communication direction for this pin
		/// </summary>
		public GPIODirection Direction
		{
			get => this._direction;
			set
			{
				if (this._direction != value) // Left to right eval ensures base class gets to check for disposed object access
				{
					// Set the direction on the pin
					BCM2835_GPIO.DirectPin(this.Pin, value);
					this._direction = value;
					if (value == GPIODirection.In)
						this.Resistor = GPIOResistor.OFF;
				}
			}
		}

        public GPIOResistor Resistor
        {
            get => this._resistor;
            set {
                if (this._resistor != value) // Left to right eval ensures base class gets to check for disposed object access
                {
					this._resistor = value;
                    BCM2835_GPIO.SetResistor(this.Pin, value);
                }
            }
        }

		public PinState State
		{
			get => this._state;
			set
			{
				if(this._state != value)
				{
					this.Write(value);
				}
			}
		}
		
		private void Write(PinState pinState)
		{
			this._state = pinState;
			BCM2835_GPIO.WritePin(this.Pin, this.State);
		}
		
		public PinState Read() {
			this._state = BCM2835_GPIO.ReadPin(this.Pin);
			return this._state;
		}

	}
}
