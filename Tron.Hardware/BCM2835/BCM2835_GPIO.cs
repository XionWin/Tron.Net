using System;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies the direction of the GPIO port
    /// </summary>
    [System.Security.SuppressUnmanagedCodeSecurity]
    class BCM2835_GPIO
    {
        public static void DirectPin(GPIOPins pin, GPIODirection direction)
        {
            bcm2835_gpio_fsel(pin, direction);
        }
        public static void WritePin(GPIOPins pin, PinState pinState)
        {
            bcm2835_gpio_write(pin, pinState);
        }
        public static PinState ReadPin(GPIOPins pin)
        {
            return bcm2835_gpio_lev(pin) ? PinState.High : PinState.Low;
        }
        public static void SetResistor(GPIOPins pin, GPIOResistor resistorMode)
        {
            bcm2835_gpio_set_pud(pin, resistorMode);
        }


        #region GPIO functions from bcm2835 library
        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_fsel")]
        protected static extern void bcm2835_gpio_fsel(GPIOPins pin, GPIODirection direction);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_write")]
        protected static extern void bcm2835_gpio_write(GPIOPins pin, PinState pinState);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_lev")]
        protected static extern bool bcm2835_gpio_lev(GPIOPins pin);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_set_pud")]
        protected static extern void bcm2835_gpio_set_pud(GPIOPins pin, GPIOResistor resistorMode);
        #endregion GPIO

    }
}