using System;

namespace Tron.Hardware
{
    public abstract class BusDevice<T>
    {
        private T _bus;
        internal T BUS
        {
            get => this._bus;
            private set => this._bus = value;
        }

        protected BusDevice(T t)
        {
            this.BUS = t;
        }
    }
}
