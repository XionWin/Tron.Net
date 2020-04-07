#define ENABLE_BUZZER
#define ENABLE_PCA9685

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

            using (Tron.Device.Indicator.IntegratedIndicator indicator = new Tron.Device.Indicator.IntegratedIndicator())
            {
                Tron.Device.Gyro.MPU9250 mpu = new Tron.Device.Gyro.MPU9250();

#if ENABLE_PCA9685
                var channels = new Tron.Device.Channel[]
                {
                    Tron.Device.Channel.Channel0,
                    Tron.Device.Channel.Channel1,
                    Tron.Device.Channel.Channel2,
                    Tron.Device.Channel.Channel3,
                };
                Tron.Device.PCA9685 pca = new Tron.Device.PCA9685(channels);
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
#if ENABLE_PCA9685
                        foreach (var channel in channels)
                        {
                            pca.SetValue(channel, i);
                        }
#endif
                        // System.Console.WriteLine(i);
                        mpu.Read();
                        // Tron.Hardware.Library.Delay(1);
                    }
                    if ((DateTime.Now - lastUpdate).TotalSeconds > 1)
                    {
                        System.Console.WriteLine("{0}", target / (DateTime.Now - start).TotalSeconds);
                        lastUpdate = DateTime.Now;
                    }
                    for (short i = target; i >= 0; i -= 1)
                    {
#if ENABLE_PCA9685
                        foreach (var channel in channels)
                        {
                            pca.SetValue(channel, i);
                        }
#endif
                        // System.Console.WriteLine(i);
                        // Tron.Hardware.Library.Delay(20);
                    }
#if ENABLE_PCA9685

#endif

                }
            }
        }
    }
}
