using System;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies Pin mask of the GPIO port
    /// </summary>
    public enum GPIOPinMask : uint
	{
		GPIO_NONE = uint.MaxValue,

		//Revision 1

		GPIO_00 = 0,
		GPIO_01 = 1,
		GPIO_04 = 1 << 4,
		GPIO_07 = 1 << 7,
		GPIO_08 = 1 << 8,
		GPIO_09 = 1 << 9,
		GPIO_10 = 1 << 10,
		GPIO_11 = 1 << 11,
		GPIO_14 = 1 << 14,
		GPIO_15 = 1 << 15,
		GPIO_17 = 1 << 17,
		GPIO_18 = 1 << 18,
		GPIO_21 = 1 << 21,
		GPIO_22 = 1 << 22,
		GPIO_23 = 1 << 23,
		GPIO_24 = 1 << 24,
		GPIO_25 = 1 << 25,

		Pin_P1_03 = 1 << 0,
		Pin_P1_05 = 1 << 1,
		Pin_P1_07 = 1 << 4,
		Pin_P1_08 = 1 << 14,
		Pin_P1_10 = 1 << 15,
		Pin_P1_11 = 1 << 17,
		Pin_P1_12 = 1 << 18,
		Pin_P1_13 = 1 << 21,
		Pin_P1_15 = 1 << 22,
		Pin_P1_16 = 1 << 23,
		Pin_P1_18 = 1 << 24,
		Pin_P1_19 = 1 << 10,
		Pin_P1_21 = 1 << 9,
		Pin_P1_22 = 1 << 25,
		Pin_P1_23 = 1 << 11,
		Pin_P1_24 = 1 << 8,
		Pin_P1_26 = 1 << 7,
		LED = 1 << 16,

		//Revision 2

		V2_GPIO_00 = 1 << 0,
		V2_GPIO_02 = 1 << 2,
		V2_GPIO_03 = 1 << 3,
		V2_GPIO_01 = 1 << 1,
		V2_GPIO_04 = 1 << 4,
		V2_GPIO_07 = 1 << 7,
		V2_GPIO_08 = 1 << 8,
		V2_GPIO_09 = 1 << 9,
		V2_GPIO_10 = 1 << 10,
		V2_GPIO_11 = 1 << 11,
		V2_GPIO_14 = 1 << 14,
		V2_GPIO_15 = 1 << 15,
		V2_GPIO_17 = 1 << 17,
		V2_GPIO_18 = 1 << 18,
		V2_GPIO_21 = 1 << 21,
		V2_GPIO_22 = 1 << 22,
		V2_GPIO_23 = 1 << 23,
		V2_GPIO_24 = 1 << 24,
		V2_GPIO_25 = 1 << 25,
		V2_GPIO_27 = 1 << 27,

		//Revision 2, new plug P5
		V2_GPIO_28 = 1 << 28,
		V2_GPIO_29 = 1 << 29,
		V2_GPIO_30 = 1 << 30,
		V2_GPIO_31 = (uint)1 << 31,

		V2_Pin_P1_03 = 1 << 2,
		V2_Pin_P1_05 = 1 << 3,
		V2_Pin_P1_07 = 1 << 4,
		V2_Pin_P1_08 = 1 << 14,
		V2_Pin_P1_10 = 1 << 15,
		V2_Pin_P1_11 = 1 << 17,
		V2_Pin_P1_12 = 1 << 18,
		V2_Pin_P1_13 = 1 << 27,
		V2_Pin_P1_15 = 1 << 22,
		V2_Pin_P1_16 = 1 << 23,
		V2_Pin_P1_18 = 1 << 24,
		V2_Pin_P1_19 = 1 << 10,
		V2_Pin_P1_21 = 1 << 9,
		V2_Pin_P1_22 = 1 << 25,
		V2_Pin_P1_23 = 1 << 11,
		V2_Pin_P1_24 = 1 << 8,
		V2_Pin_P1_26 = 1 << 7,
		V2_LED = 1 << 16,

		//Revision 2, new plug P5
		V2_Pin_P5_03 = 1 << 28,
		V2_Pin_P5_04 = 1 << 29,
		V2_Pin_P5_05 = 1 << 30,
		V2_Pin_P5_06 = (uint)1 << 31,

	}
}