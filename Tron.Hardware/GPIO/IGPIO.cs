using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    public interface IGPIO
    {
        #region Properties
        /// <summary>
        /// The currently assigned GPIO pin.
        /// </summary>
        GPIOPins Pin { get; }

        /// <summary>
        /// Gets the bit mask of this pin.
        /// </summary>
        GPIOPinMask Mask { get; }

        /// <summary>
        /// Direction of the GPIO pin
        /// </summary>
        GPIODirection Direction { get; set; }

        /// <summary>
        /// GPIO pull up-down resistor
        /// </summary>
        // BCM2835_GPIO_PUD_OFF = 0b00 = 0
        // BCM2835_GPIO_PUD_DOWN = 0b01 = 1
        // BCM2835_GPIO_PUD_UP = 0b10 = 2
        GPIOResistor Resistor { get; set; }

        /// <summary>
        /// Gets the state of this pin.
        /// </summary>
        PinState State { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Read a value from the pin
        /// </summary>
        /// <returns>The value read from the pin</returns>
        PinState Read();
        #endregion
    }
}