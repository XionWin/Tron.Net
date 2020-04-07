using System;

namespace Tron.Hardware
{
    /// <summary>
    /// Specifies Pin of the GPIO port
    /// </summary>
	public enum GPIOPins : byte
	{
		GPIO_NONE = 255,

		//Revision 1

		GPIO_00 = 0,
		GPIO_01 = 1,
		GPIO_04 = 4,
		GPIO_07 = 7,
		GPIO_08 = 8,
		GPIO_09 = 9,
		GPIO_10 = 10,
		GPIO_11 = 11,
		GPIO_13 = 13,
		GPIO_14 = 14,
		GPIO_15 = 15,
		GPIO_17 = 17,
		GPIO_18 = 18,
		GPIO_19 = 19,
		GPIO_20 = 20,
		GPIO_21 = 21,
		GPIO_22 = 22,
		GPIO_23 = 23,
		GPIO_24 = 24,
		GPIO_25 = 25,
		GPIO_26 = 26,

		Pin_P1_03 = 0,
		Pin_P1_05 = 1,
		Pin_P1_07 = 4,
		Pin_P1_08 = 14,
		Pin_P1_10 = 15,
		Pin_P1_11 = 17,
		Pin_P1_12 = 18,
		Pin_P1_13 = 21,
		Pin_P1_15 = 22,
		Pin_P1_16 = 23,
		Pin_P1_18 = 24,
		Pin_P1_19 = 10,
		Pin_P1_21 = 9,
		Pin_P1_22 = 25,
		Pin_P1_23 = 11,
		Pin_P1_24 = 8,
		Pin_P1_26 = 7,
		LED = 16,

		//Revision 2

		V2_GPIO_00 = 0,
		V2_GPIO_02 = 2,
		V2_GPIO_03 = 3,
		V2_GPIO_01 = 1,
		V2_GPIO_04 = 4,
		V2_GPIO_07 = 7,
		V2_GPIO_08 = 8,
		V2_GPIO_09 = 9,
		V2_GPIO_10 = 10,
		V2_GPIO_11 = 11,
		V2_GPIO_14 = 14,
		V2_GPIO_15 = 15,
		V2_GPIO_17 = 17,
		V2_GPIO_18 = 18,
		V2_GPIO_21 = 21,
		V2_GPIO_22 = 22,
		V2_GPIO_23 = 23,
		V2_GPIO_24 = 24,
		V2_GPIO_25 = 25,
		V2_GPIO_27 = 27,

		//Revision 2, new plug P5
		V2_GPIO_28 = 28,
		V2_GPIO_29 = 29,
		V2_GPIO_30 = 30,
		V2_GPIO_31 = 31,

		V2_Pin_P1_03 = 2,
		V2_Pin_P1_05 = 3,
		V2_Pin_P1_07 = 4,
		V2_Pin_P1_08 = 14,
		V2_Pin_P1_10 = 15,
		V2_Pin_P1_11 = 17,
		V2_Pin_P1_12 = 18,
		V2_Pin_P1_13 = 27,
		V2_Pin_P1_15 = 22,
		V2_Pin_P1_16 = 23,
		V2_Pin_P1_18 = 24,
		V2_Pin_P1_19 = 10,
		V2_Pin_P1_21 = 9,
		V2_Pin_P1_22 = 25,
		V2_Pin_P1_23 = 11,
		V2_Pin_P1_24 = 8,
		V2_Pin_P1_26 = 7,
		V2_LED = 16,

		//Revision 2, new plug P5
		V2_Pin_P5_03 = 28,
		V2_Pin_P5_04 = 29,
		V2_Pin_P5_05 = 30,
		V2_Pin_P5_06 = 31,
		
		//A+ and B+ pins
		V2Plus_Pin_P1_29 = 5,
		V2Plus_Pin_P1_31 = 6,
		V2Plus_Pin_P1_32 = 12,
		V2Plus_Pin_P1_33 = 13,
		V2Plus_Pin_P1_35 = 19,
		V2Plus_Pin_P1_36 = 16,
		V2Plus_Pin_P1_37 = 26,
		V2Plus_Pin_P1_38 = 20,
		V2Plus_Pin_P1_40 = 21,

	}
}