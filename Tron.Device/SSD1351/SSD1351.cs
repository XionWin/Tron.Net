using System;

namespace Tron.Device
{
    public class SSD1351: BusDevice<Hardware.ISPI>
    {
        private const Hardware.GPIOPins PIN_CS = Hardware.GPIOPins.GPIO_08;
        private const Hardware.GPIOPins PIN_RST = Hardware.GPIOPins.GPIO_25;
        private const Hardware.GPIOPins PIN_DC = Hardware.GPIOPins.GPIO_24;

        private const byte  X_MAXPIXEL = 128;                           //OLED width maximum memory 
        private const byte Y_MAXPIXEL = 128;                            //OLED height maximum memory
        private const byte  X_OFFSET = 0;
        private const byte Y_OFFSET = 0;

        private const byte SCREEN_WIDTH = (X_MAXPIXEL - 2 * X_OFFSET);  //OLED width
        private const byte SCREEN_HEIGHT = Y_MAXPIXEL;                  //OLED height
        
        public Hardware.PinState CS
        {
            set
            {
                Hardware.GPIO.CreatePin(PIN_CS).State = value;
            }
        }

        private Hardware.PinState _rst = Hardware.PinState.Low;
        public Hardware.PinState RST
        {
            get => this._rst;
            set
            {
                Hardware.GPIO.CreatePin(PIN_RST).State = value;
            }
        }

        public Hardware.PinState DC
        {
            set
            {
                Hardware.GPIO.CreatePin(PIN_DC).State = value;
            }
        }

        public OLEDDeviceInfo DeviceInfo
        {
            get;
        } = new OLEDDeviceInfo()
        {
            Direction = OLEDScanDirection.D2U_R2L,
            Width = SCREEN_WIDTH,
            Height = SCREEN_HEIGHT,
            XOffset = X_OFFSET,
            YOffset = Y_OFFSET
        };

        private byte[] _frameBuffer;
        public byte[] FrameBuffer
        {
            get => this._frameBuffer;
            set
            {
                Array.Copy(value, this._frameBuffer, Math.Min(value.Length, this._frameBuffer.Length));
                this.Display();
            }
        }

        public SSD1351(Hardware.ISPI spi): base(spi)
        {
            Init();
        }

        public bool Init()
        {
            this._frameBuffer = new byte[SCREEN_WIDTH / 2 * SCREEN_HEIGHT];

            Reset();
            var result = InitBus();
            result &= InitReg();
 

            //Set the display scan and color transfer modes
            SetGramScanWay(this.DeviceInfo.Direction, this.DeviceInfo.Width, this.DeviceInfo.Height, this.DeviceInfo.XOffset, this.DeviceInfo.YOffset);
            Hardware.Library.Delay(10);
            //Turn on the OLED display
            this.WriteReg(0xAF);
            Clear(0x00);
            Hardware.Library.Delay(10);

            return result;
        }
        void Reset()
        {
            RST = Hardware.PinState.High;
            Hardware.Library.Delay(10);
            RST = Hardware.PinState.Low;
            Hardware.Library.Delay(10);
            RST = Hardware.PinState.High;
            Hardware.Library.Delay(10);
        }
        
        bool InitBus()
        {
            this.BUS.BitOrder =  Hardware.SPIBitOrder.MSBFIRST;                        //High first transmission
            this.BUS.Mode =  Hardware.SPIMode.MODE0;                                   //spi mode 1
            this.BUS.ClockDivider =  Hardware.SPIClockDivider.CLOCK_DIVIDER_16;        //Frequency
            this.BUS.CS =  Hardware.SPIChipSelect.CS0;                                 //set CE0
            this.BUS.SetChipSelectPolarity(this.BUS.CS, false);                             //enable cs0
            return true;
        }

        bool InitReg()
        {
            this.WriteReg(0xAE);    //--turn off oled panel

            this.WriteReg(0xa0);    //gment remap
            this.WriteReg(0x51);    //51

            this.WriteReg(0xa1);    //start line
            this.WriteReg(0x00);

            this.WriteReg(0xa2);    //display offset
            this.WriteReg(0x00);

            this.WriteReg(0xa4);    //rmal display

            this.WriteReg(0xa8);    //set multiplex ratio
            this.WriteReg(0x7f);
            
            // this.WriteReg(0xB8);    //   set gray scale table
            // this.WriteReg(0x01);    //   Gray Scale Level 1
            // this.WriteReg(0x11);    //   Gray Scale Level 3 & 2
            // this.WriteReg(0x11);    //   Gray Scale Level 5 & 4
            // this.WriteReg(0x11);    //   Gray Scale Level 7 & 6
            // this.WriteReg(0x11);    //   Gray Scale Level 9 & 8
            // this.WriteReg(0x11);    //   Gray Scale Level 11 & 10
            // this.WriteReg(0x11);    //   Gray Scale Level 13 & 12
            // this.WriteReg(0x11);    //   Gray Scale Level 15 & 14

            
            this.WriteReg(0xB9);    //   set gray scale table
            this.WriteReg(0xFF);        
            

            this.WriteReg(0xb3);    //set dclk
            this.WriteReg(0x00);    //80Hz:0xc1 90Hz:0xe1   100Hz:0x00   110Hz:0x30 120Hz:0x50   130Hz:0x70     01

            this.WriteReg(0xab);    //Enable Internal Vdd
            this.WriteReg(0x01);

            this.WriteReg(0xb1);    //set phase leghth
            this.WriteReg(0x22);

            this.WriteReg(0xbc);    //Pre-charge voltage: Vcomh
            this.WriteReg(0x08);
            
            this.WriteReg(0xbe);    //COM deselect voltage level: 0.86 x Vcc
            this.WriteReg(0x0f);

            this.WriteReg(0xd5);    //Enable 2nd pre-charge
            this.WriteReg(0x62);

            this.WriteReg(0xb6);    //set phase leghth
            this.WriteReg(0x0f);

            this.WriteReg(0x15);    //set column address
            this.WriteReg(0x00);    //start column   0
            this.WriteReg(0x7f);    //end column   127

            this.WriteReg(0x75);    //set row address
            this.WriteReg(0x00);    //start row   0
            this.WriteReg(0x7f);    //end row   127

            this.WriteReg(0x81);    //set contrast control
            this.WriteReg(0x80);


            this.WriteReg(0xfd);
            this.WriteReg(0x12);

            return true;
        }

        void SetGramScanWay(OLEDScanDirection direction, byte width, byte height, byte xOffset, byte yOffset)
        {
            if(direction == OLEDScanDirection.L2R_U2D){
                this.WriteReg(0xa0);    //gment remap
                this.WriteReg(0x51);
                //this.WriteReg(0xa7);
            }else if(direction == OLEDScanDirection.L2R_D2U){//Y
                this.WriteReg(0xa0);    //gment remap
                this.WriteReg(0x41);
            }else if(direction == OLEDScanDirection.R2L_U2D){
                this.WriteReg(0xa0);    //gment remap
                this.WriteReg(0x52);
            }else if(direction == OLEDScanDirection.R2L_D2U){
                this.WriteReg(0xa0);    //gment remap
                this.WriteReg(0x42);
            }
            else if(direction == OLEDScanDirection.U2D_L2R){
                this.WriteReg(0xa0);    //gment remap
                this.WriteReg(0x51);
            }else if(direction == OLEDScanDirection.U2D_R2L){
                this.WriteReg(0xa0);    //gment remap
                this.WriteReg(0x51);
            }else if(direction == OLEDScanDirection.D2U_L2R){
                this.WriteReg(0xa0);    //gment remap
                this.WriteReg(0x41);
            }else if(direction == OLEDScanDirection.D2U_R2L){//Y
                this.WriteReg(0xa0);    //gment remap
                this.WriteReg(0x42);
            }
            else{
                return;
            }
        }
        void SetWindow(byte Xstart, byte Ystart, byte Xend, byte Yend)
        {
            if((Xstart > this.DeviceInfo.Width) || (Ystart > this.DeviceInfo.Height) ||
            (Xend > this.DeviceInfo.Width) || (Yend > this.DeviceInfo.Height))
                return;

            this.WriteReg(0x15);
            this.WriteReg(Xstart);
            this.WriteReg(--Xend);

            this.WriteReg(0x75);
            this.WriteReg(Ystart);
            this.WriteReg(--Yend);
        }


        void WriteReg(byte value)
        {
            DC = Hardware.PinState.Low;
            CS = Hardware.PinState.Low;
            this.BUS.Write(value);
            CS = Hardware.PinState.High;
        }
        void WriteData(byte value)
        {
            DC = Hardware.PinState.High;
            CS = Hardware.PinState.Low;
            this.BUS.Write(value);
            CS = Hardware.PinState.High;
        }

        void WriteData(byte[] data, int len)
        {
            DC = Hardware.PinState.High;
            CS = Hardware.PinState.Low;
            this.BUS.Write(data, len);
            CS = Hardware.PinState.High;
        }


        public void Clear(ushort Color)
        {
            for(var i = 0; i < this._frameBuffer.Length; i++) {
                this._frameBuffer[i] = (byte)(Color | (Color << 4));
            }
            this.Display();
        }

        void Display()
        {
            SetWindow(0, 0, this.DeviceInfo.Width, this.DeviceInfo.Height);
            this.WriteData(this._frameBuffer, this._frameBuffer.Length);
        }
        

    }
}
