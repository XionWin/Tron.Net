// #define ENABLE_BUZZER
// #define ENABLE_MOTOR

using System;
using System.Threading;

namespace Quadcopter
{
    class Program
    {
        static void Main(string[] args)
        {
            Tron.Linux.Realtime.SetRealtime();
            // initialize the mapped memory
            if (!Tron.Hardware.Library.Init())
                throw new Exception("Unable to initialize bcm2835.so library");
            MachineInfo.Show();

#if ENABLE_BUZZER
            using (Tron.Device.Indicator.IntegratedIndicator indicator = new Tron.Device.Indicator.IntegratedIndicator())
#else
            using (Tron.Device.Indicator.IntegratedIndicator indicator = new Tron.Device.Indicator.IntegratedIndicator(true, false))
#endif
            {
                var gyro = new Tron.Device.Gyro.MPU9250.Module();

#if ENABLE_MOTOR
                var channels = new Tron.Device.Channel[]
                {
                    Tron.Device.Channel.Channel0,
                    Tron.Device.Channel.Channel1,
                    Tron.Device.Channel.Channel2,
                    Tron.Device.Channel.Channel3,
                };
                var pca = new Tron.Device.MotorControl.PCA9685.Module(channels);
                pca.Enable = true;
#endif
                indicator.Status = Tron.Device.Indicator.IndicatorStatus.STANDBY;
                Tron.Hardware.Library.Delay(1000);

                indicator.Status = Tron.Device.Indicator.IndicatorStatus.RUNING;

                DateTime lastUpdate = DateTime.Now;
                while (true)
                {
                    var start = DateTime.Now;
                    short target = 50;
                    for (short i = 0; i < target; i += 1)
                    {
#if ENABLE_MOTOR
                        foreach (var channel in channels)
                        {
                            pca.SetValue(channel, i);
                        }
#endif
                        // System.Console.WriteLine(i);
                        gyro.Read();
                        // Tron.Hardware.Library.Delay(1);
                    }
                    var counter = target / (DateTime.Now - start).TotalSeconds;
                    if (counter < 7000)
                    {
                        System.Console.WriteLine(counter);
                        indicator.Status = Tron.Device.Indicator.IndicatorStatus.WRINING;
                    }
                    else
                    {
                        indicator.Status = Tron.Device.Indicator.IndicatorStatus.RUNING;
                    }
                    // if ((DateTime.Now - lastUpdate).TotalSeconds > 1)
                    // {
                    //     System.Console.WriteLine("{0}", target / (DateTime.Now - start).TotalSeconds);
                    //     lastUpdate = DateTime.Now;
                    // }
                    for (short i = target; i >= 0; i -= 1)
                    {
#if ENABLE_MOTOR
                        foreach (var channel in channels)
                        {
                            pca.SetValue(channel, i);
                        }
#endif
                        // System.Console.WriteLine(i);
                        // Tron.Hardware.Library.Delay(20);
                    }
#if ENABLE_MOTOR

#endif

                }
            }
        }
    }
}
