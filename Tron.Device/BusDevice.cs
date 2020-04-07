using System;

namespace Tron.Device
{
    public abstract class BusDevice<T>
    {
        private T _bus;
        public T BUS
        {
            get => this._bus;
            protected set => this._bus = value;
        }
        

    }
}
