using System;
using System.Runtime.InteropServices;

namespace Quadcopter
{
	class MachineInfo
	{
		public static void Show()
		{
#if DEBUG
			System.Console.WriteLine("----------------------------------------------------------------");
			System.Console.WriteLine("OS: {0}", Tron.Core.Machine.OSDescription);
			System.Console.WriteLine("Framework: {0}", Tron.Core.Machine.FrameworkDescription);
			System.Console.WriteLine("OS architecture: {0}", Tron.Core.Machine.OSArchitecture);
			System.Console.WriteLine("Process architecture: {0}", Tron.Core.Machine.ProcessArchitecture);
			System.Console.WriteLine("Library version: {0}", Tron.Hardware.Library.Version);
			System.Console.WriteLine("UpTime: {0}", Tron.Core.Machine.UpTime);
			System.Console.WriteLine("----------------------------------------------------------------");
#endif
		}
	}

}
