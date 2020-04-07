using System;
using System.Collections.Generic;

namespace Tron.Device.Indicator
{
    internal enum IndicatorColor: byte
    {
        NULL = 0x00,
        RED = 0x01,
        GREEN = 0x02,
        BLUE = 0x04,
        YELLOW = 0x03,
        CYAN = 0x06,
        MAGENTA = 0x05,
        WHITE = 0x07,
    }
}