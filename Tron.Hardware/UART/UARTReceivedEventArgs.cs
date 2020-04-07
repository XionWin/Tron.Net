using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tron.Hardware
{
	public class UARTReceivedEventArgs: EventArgs
	{
		public UARTReceivedEventArgs()
		{
		}
		public UARTReceivedEventArgs(IEnumerable<byte> data)
		{
			this.Data = data;
		}

		public IEnumerable<byte> Data
		{
			get;
			internal set;
		}
	}
}
