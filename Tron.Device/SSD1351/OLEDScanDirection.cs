using System;

namespace Tron.Device
{
    /// <summary>
    /// Specifies the direction of the GPIO port
    /// </summary>
    public enum OLEDScanDirection : byte
	{ 
		L2R_U2D = 0,	//The display interface is displayed , left to right, up to down 
		L2R_D2U = 1,
		R2L_U2D = 2,
		R2L_D2U = 3,
		
		U2D_L2R = 4,
		U2D_R2L = 5,
		D2U_L2R = 6,
		D2U_R2L = 7, 
	}
}