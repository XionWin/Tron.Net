#define ENABLE_BUZZER
#define ENABLE_MOTOR

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

#if ENABLE_BUZZER
            using (var indicator = new Tron.Device.Indicator.Module())
#else
            using (var indicator = new Tron.Device.Indicator.Module(true, false))
#endif
            {
                var gyro = new Tron.Device.Gyro.MPU9250.Module();

                System.Console.WriteLine("Calibrate gyro module...");
                var c = gyro.Calibrate();
                System.Console.WriteLine
                (
                    "gx: {0} gy: {1} gz: {2}\nax: {3} ay: {4} az: {5}",
                    c.gx,
                    c.gy,
                    c.gz,
                    c.ax,
                    c.ay,
                    c.az
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
                        gyro.Read();
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
                    if ((DateTime.Now - lastUpdate).TotalSeconds > 1)
                    {
                        System.Console.WriteLine("{0}", minCounter);
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
