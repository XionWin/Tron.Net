using System;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies up-down resistor of the GPIO prot
    /// </summary>
    public enum GPIOResistor : byte
    {
        OFF = 0,
        PULL_DOWN = 1,
        PULL_UP = 2
    }
}