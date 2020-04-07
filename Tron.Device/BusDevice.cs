using System;

namespace Tron.Device
{
    public abstract class BusDevice<T>
    {
        private T _bus;

        public BusDevice(T bus)
        {
            this._bus = bus;
        }
        
        internal T BUS
        {
            get => this._bus;
        }
        

    }
}
