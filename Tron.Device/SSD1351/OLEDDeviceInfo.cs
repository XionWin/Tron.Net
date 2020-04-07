using System;

namespace Tron.Device
{
    public struct OLEDDeviceInfo
	{
		public byte Width
		{
			get;
			set;
		}
		public byte Height
		{
			get;
			set;
		}
		public OLEDScanDirection Direction
		{
			get;
			set;
		}
		
		//OLED x actual display position calibration
		public byte XOffset	
		{
			get;
			set;
		}
		//OLED y actual display position calibration
		public byte YOffset
		{
			get;
			set;
		}		
	}
}