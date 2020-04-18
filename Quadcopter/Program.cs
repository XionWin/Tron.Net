#define ENABLE_LED
#define ENABLE_BUZZER
// #define ENABLE_MOTOR

using System;

namespace Quadcopter
{
    class Program
    {
        static void Main(string[] args)
        {
            Tron.Linux.System.SetRealtime();
            // initialize the mapped memory
            if (!Tron.Hardware.Library.Init())
                throw new Exception("Unable to initialize bcm2835 library");
            MachineInfo.Show();

            using (var indicator = new Tron.Device.Indicator.Module())
            {

#if !ENABLE_LED
                indicator.LEDSwitch = false;
#endif
#if !ENABLE_BUZZER
                indicator.BuzzerSwitch = false;
#endif

                indicator.Status = Tron.Device.Indicator.IndicatorStatus.INIT;

                var gyro = new Tron.Device.Gyro.MPU9250.Module();


                // System.Console.WriteLine("Calibrate gyro module...");
                // gyro.Calibrate();
                // System.Console.WriteLine
                // (
                //     "ax: {0} ay: {1} az: {2}\ngx: {3} gy: {4} gz: {5}",
                //     gyro.AccBias.X,
                //     gyro.AccBias.Y,
                //     gyro.AccBias.Z,
                //     gyro.GyroBias.X,
                //     gyro.GyroBias.Y,
                //     gyro.GyroBias.Z
                // );

                // System.Console.WriteLine("Wave device in a figure eight until done");
                // gyro.CalibrateSlave();
                // System.Console.WriteLine
                // (
                //     "mbx: {0} mby: {1} az: {2}\nmsx: {3} msy: {4} msz: {5}",
                //     gyro.MagBias.X,
                //     gyro.MagBias.Y,
                //     gyro.MagBias.Z,
                //     gyro.MagScale.X,
                //     gyro.MagScale.Y,
                //     gyro.MagScale.Z
                // );
                // System.Console.WriteLine("Mag Calibration done!");

                gyro.Initiailze();
                gyro.InitiailzeSlave();

                // System.Console.WriteLine("Wave device in a figure eight until done");
                // gyro.CalibrateSlave();
                // System.Console.WriteLine
                // (
                //     "mbx: {0} mby: {1} az: {2}\nmsx: {3} msy: {4} msz: {5}",
                //     gyro.MagBias.X,
                //     gyro.MagBias.Y,
                //     gyro.MagBias.Z,
                //     gyro.MagScale.X,
                //     gyro.MagScale.Y,
                //     gyro.MagScale.Z
                // );
                // System.Console.WriteLine("Mag Calibration done!");

#if ENABLE_MOTOR
                var channels = new Tron.Device.MotorControl.PCA9685.Channel[]
                {
                    Tron.Device.MotorControl.PCA9685.Channel.Channel0,
                    Tron.Device.MotorControl.PCA9685.Channel.Channel1,
                    Tron.Device.MotorControl.PCA9685.Channel.Channel2,
                    Tron.Device.MotorControl.PCA9685.Channel.Channel3,
                };
                var motor = new Tron.Device.MotorControl.PCA9685.Module(channels);
                motor.Enable = true;
#endif


                indicator.Status = Tron.Device.Indicator.IndicatorStatus.STANDBY;
                Tron.Hardware.Library.Delay(1000);


                indicator.Status = Tron.Device.Indicator.IndicatorStatus.RUNING;



                DateTime lastUpdate = DateTime.Now;
                long min_sigle_counter = long.MaxValue;
                double total_counter = 0;
                Tron.Core.Data.EularAngles eular = new Tron.Core.Data.EularAngles();
                
                new Tron.Flight.AHRS(gyro, (ahrs) =>
                {

                   var start = DateTime.Now;
                   //                     short target = 200;
                   //                     for (short i = 0; i < target; i += 1)
                   //                     {
                   // #if ENABLE_MOTOR
                   //                         foreach (var channel in channels)
                   //                         {
                   //                             motor.SetValue(channel, i);
                   //                         }
                   // #endif
                   //                         eular = read(gyro);
                   //                         Tron.Linux.System.Sleep(50);
                   //                     }

                   eular = ahrs.EularAngles;
                   byte target = (byte)(Math.Abs(eular.Roll) * 10);
#if ENABLE_MOTOR
                    foreach (var channel in channels)
                    {
                        motor.SetValue(channel, target);
                    }
#endif
                   Tron.Linux.System.Sleep(50);
                   total_counter++;

                   min_sigle_counter = Math.Min(1000 * 10000 / (DateTime.Now - start).Ticks, min_sigle_counter);

                   if ((DateTime.Now - lastUpdate).TotalMilliseconds > 20)
                   {
                       // System.Console.Write
                       // (
                       //     "ax: {0} ay: {1} az: {2}\t",
                       //     _Accel.X.ToString("N2").PadLeft(8, ' '),
                       //     _Accel.Y.ToString("N2").PadLeft(8, ' '),
                       //     _Accel.Z.ToString("N2").PadLeft(8, ' ')
                       // );

                       // System.Console.Write
                       // (
                       //     "gx: {0} gy: {1} gz: {2}\t",
                       //     _Gyro.X.ToString("N2").PadLeft(8, ' '),
                       //     _Gyro.Y.ToString("N2").PadLeft(8, ' '),
                       //     _Gyro.Z.ToString("N2").PadLeft(8, ' ')
                       // );

                       // System.Console.Write
                       // (
                       //     "mx: {0} my: {1} mz: {2}\t",
                       //     _Mag.X.ToString("N2").PadLeft(8, ' '),
                       //     _Mag.Y.ToString("N2").PadLeft(8, ' '),
                       //     _Mag.Z.ToString("N2").PadLeft(8, ' ')
                       // );

                       System.Console.Write
                       (
                           "PITCH: {0} ROLL: {1} YAW: {2}\t",
                           eular.Pitch.ToString("N2").PadLeft(7, ' '),
                           eular.Roll.ToString("N2").PadLeft(7, ' '),
                           eular.Yaw.ToString("N2").PadLeft(7, ' ')
                       );


                       var count = total_counter / (DateTime.Now - lastUpdate).TotalSeconds;
                       if (count < 2000 || min_sigle_counter < 500)
                       {
                           indicator.Status = Tron.Device.Indicator.IndicatorStatus.WRINING;
                       }
                       else
                       {
                           indicator.Status = Tron.Device.Indicator.IndicatorStatus.RUNING;
                       }


                       System.Console.WriteLine("Frequency: {0}, Min: {1}", count.ToString().PadLeft(4, ' '), min_sigle_counter.ToString().PadLeft(4, ' '));
                       lastUpdate = DateTime.Now;
                       min_sigle_counter = long.MaxValue;
                       total_counter = 0;
                   }

                   //                     for (short i = target; i >= 0; i -= 1)
                   //                     {
                   // #if ENABLE_MOTOR
                   //                         foreach (var channel in channels)
                   //                         {
                   //                             motor.SetValue(channel, i);
                   //                         }
                   // #endif
                   //                     }





               });

            }
        }

    }
}
