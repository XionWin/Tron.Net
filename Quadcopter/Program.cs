// #define ENABLE_LED
#define ENABLE_BUZZER
// #define ENABLE_MOTOR

using System;
using System.Threading;

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

                var gyro = new Tron.Device.Gyro.MPU9250.Module();

                System.Console.WriteLine("Calibrate gyro module...");
                var c = gyro.Calibrate();
                System.Console.WriteLine
                (
                    "ax: {0} ay: {1} az: {2}\ngx: {3} gy: {4} gz: {5}",
                    c.ax,
                    c.ay,
                    c.az,
                    c.gx,
                    c.gy,
                    c.gz
                );

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
                double minCounter = double.MaxValue;

                Tron.Core.Data.Vector3 acc = new Tron.Core.Data.Vector3();
                Tron.Core.Data.Vector3 g = new Tron.Core.Data.Vector3();
                while (true)
                {
                    var start = DateTime.Now;
                    short target = 20;
                    for (short i = 0; i < target; i += 1)
                    {
#if ENABLE_MOTOR
                        foreach (var channel in channels)
                        {
                            motor.SetValue(channel, i);
                        }
#endif
                        acc = gyro.Accel;
                        g = gyro.Gyro;
                        Tron.Linux.System.Sleep(50);
                    }

                    minCounter = Math.Min(target / (DateTime.Now - start).TotalSeconds, minCounter);
                    if (minCounter < 3000)
                    {
                        indicator.Status = Tron.Device.Indicator.IndicatorStatus.WRINING;
                    }
                    else
                    {
                        indicator.Status = Tron.Device.Indicator.IndicatorStatus.RUNING;
                    }
                    if ((DateTime.Now - lastUpdate).TotalMilliseconds > 1000)
                    {
                        // System.Console.Write
                        // (
                        //     "ax: {0} ay: {1} az: {2}\t",
                        //     acc.X.ToString().PadLeft(6, ' '),
                        //     acc.Y.ToString().PadLeft(6, ' '),
                        //     acc.Z.ToString().PadLeft(6, ' ')
                        // );
                        // System.Console.Write
                        // (
                        //     "gx: {0} gy: {1} gz: {2}\t",
                        //     g.X.ToString().PadLeft(6, ' '),
                        //     g.Y.ToString().PadLeft(6, ' '),
                        //     g.Z.ToString().PadLeft(6, ' ')
                        // );
                        System.Console.WriteLine("Frequency: {0}", minCounter.ToString().PadLeft(4, ' '));
                        lastUpdate = DateTime.Now;
                        minCounter = double.MaxValue;
                    }

                    for (short i = target; i >= 0; i -= 1)
                    {
#if ENABLE_MOTOR
                        foreach (var channel in channels)
                        {
                            motor.SetValue(channel, i);
                        }
#endif
                    }
#if ENABLE_MOTOR

#endif

                }
            }
        }
    }
}
