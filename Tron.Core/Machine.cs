using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tron.Core
{
	public class Machine
	{
		public static string OSDescription
		{
			get => RuntimeInformation.OSDescription;
		}
		public static string FrameworkDescription
		{
			get => RuntimeInformation.FrameworkDescription;
		}
		public static Architecture OSArchitecture
		{
			get => RuntimeInformation.OSArchitecture;
		}
		public static Architecture ProcessArchitecture
		{
			get => RuntimeInformation.ProcessArchitecture;
		}
		public static TimeSpan UpTime
		{
			get => new TimeSpan((long)System.Environment.TickCount * 10000);
		}

	}
}
