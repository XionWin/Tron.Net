#define ENABLE_LED
// #define ENABLE_BUZZER
// #define ENABLE_MOTOR

using System;

namespace Quadcopter
{
    class Program
    {
        public const double KP = 2.2d;
        public const double KI = 2d;
        public const double KD = 0.2d;
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
                    Tron.Device.MotorControl.PCA9685.Channel.Channel1,
                    Tron.Device.MotorControl.PCA9685.Channel.Channel2,
                    Tron.Device.MotorControl.PCA9685.Channel.Channel3,
                    Tron.Device.MotorControl.PCA9685.Channel.Channel0,
                };
                var motor = new Tron.Device.MotorControl.PCA9685.Module(channels);
                motor.Enable = true;
#endif



                ushort thrust = 0;
                double angle = 0d;
                bool reset_angle = false;
                var udp = new Tron.Net.UdpClient(
                    (data) =>
                    {
                        if (data.Length == 1 & data[0] == 0xFF)
                        {
#if ENABLE_MOTOR
                            motor.Enable = false;
#endif
                        }
                        if (data.Length == 1 & data[0] == 0xFE)
                        {
                            reset_angle = true;
                        }
                        else if (data.Length == 2)
                        {
                            thrust = Convert.ToUInt16((double)((data[0] << 8) + data[1]) / 32767 * 800);
                        }
                    }
                );

                udp.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("192.168.0.15"), 8815));

                indicator.Status = Tron.Device.Indicator.IndicatorStatus.STANDBY;
                Tron.Hardware.Library.Delay(1000);


                indicator.Status = Tron.Device.Indicator.IndicatorStatus.RUNING;



                var start_delay = new TimeSpan(0, 0, 10);
                var start = DateTime.Now;

                var xPIDCtrl = new Tron.Flight.PIDController(KP, KI, KD);
                var yPIDCtrl = new Tron.Flight.PIDController(KP, KI, KD);
                var zPIDCtrl = new Tron.Flight.PIDController(KP, KI, KD);

                double yawMemory = -1;

                byte[] sendData = new byte[9];
                new Tron.Flight.AHRS(gyro, (ahrs) =>
                {
                    if (yawMemory < 0)
                    {
                        yawMemory = ahrs.EularAngles.Yaw;
                    }

                    var td = (DateTime.Now - start).TotalSeconds;

                    var latestPitch = Math.Min(Math.Max(ahrs.EularAngles.Pitch / 360d, -1d), 1d);
                    var latestRoll = Math.Min(Math.Max(ahrs.EularAngles.Roll / 360d, -1d), 1d);
                    var latestYaw = Math.Min(Math.Max(ahrs.EularAngles.Yaw / 360d, -1d), 1d);

                    latestYaw = latestYaw > 0.5 ? 1 - latestYaw : latestYaw;
                    latestYaw = -latestYaw;

                    var pitch = yPIDCtrl.Manipulate(0, latestPitch, td);
                    var roll = xPIDCtrl.Manipulate(0, latestRoll, td);
                    if (reset_angle)
                    {
                        angle = latestYaw;
                        // reset_angle = false;
                    }
                    var yaw = zPIDCtrl.Manipulate(angle, latestYaw, td);


                    if (reset_angle)
                    {
                        yaw = 0;
                    }

                    var thrust_real = thrust;

                    double frontLeftSpeed = thrust_real *
                        (pitch > 0d ? 1d : 1d - -pitch) *
                        (roll > 0d ? 1d : 1d - -roll) *
                        (yaw > 0d ? 1d : 1d - -yaw);

                    double frontRightSpeed = thrust_real *
                        (pitch > 0d ? 1d : 1d - -pitch) *
                        (roll < 0d ? 1d : 1d - roll) *
                        (yaw < 0d ? 1d : 1d - yaw);

                    double rearRightSpeed = thrust_real *
                        (pitch < 0d ? 1d : 1d - pitch) *
                        (roll < 0d ? 1d : 1d - roll) *
                        (yaw > 0d ? 1d : 1d - -yaw);

                    double rearLeftSpeed = thrust_real *
                        (pitch < 0d ? 1d : 1d - pitch) *
                        (roll > 0d ? 1d : 1d - -roll) *
                        (yaw < 0d ? 1d : 1d - yaw);

                    frontLeftSpeed = (short)Math.Min((ushort)frontLeftSpeed, thrust);
                    frontRightSpeed = (short)Math.Min((ushort)frontRightSpeed, thrust);
                    rearRightSpeed = (short)Math.Min((ushort)rearRightSpeed, thrust);
                    rearLeftSpeed = (short)Math.Min((ushort)rearLeftSpeed, thrust);


                    System.Console.Write(ahrs.EularAngles);
                    System.Console.WriteLine
                    (
                        "\t {0} {1} {2} {3}",
                        frontLeftSpeed.ToString().PadLeft(4),
                        frontRightSpeed.ToString().PadLeft(4),
                        rearRightSpeed.ToString().PadLeft(4),
                        rearLeftSpeed.ToString().PadLeft(4)
                    );


#if ENABLE_MOTOR

                    motor.SetValue(channels[0], (short)Math.Min((ushort)frontLeftSpeed, thrust));
                    motor.SetValue(channels[1], (short)Math.Min((ushort)frontRightSpeed, thrust));
                    motor.SetValue(channels[2], (short)Math.Min((ushort)rearRightSpeed, thrust));
                    motor.SetValue(channels[3], (short)Math.Min((ushort)rearLeftSpeed, thrust));
#endif

                    Tron.Linux.System.Sleep(50);

                    sendData[0] = Convert.ToByte(ahrs.EularAngles.Pitch < 0 ? 0x01 : 0x00);
                    sendData[1] = Convert.ToByte((ushort)Math.Abs(ahrs.EularAngles.Pitch) >> 8);
                    sendData[2] = Convert.ToByte((byte)Math.Abs(ahrs.EularAngles.Pitch));
                    sendData[3] = Convert.ToByte(ahrs.EularAngles.Roll < 0 ? 0x01 : 0x00);
                    sendData[4] = Convert.ToByte((ushort)Math.Abs(ahrs.EularAngles.Roll) >> 8);
                    sendData[5] = Convert.ToByte((byte)Math.Abs(ahrs.EularAngles.Roll));
                    sendData[6] = Convert.ToByte(ahrs.EularAngles.Yaw < 0 ? 0x01 : 0x00);
                    sendData[7] = Convert.ToByte((ushort)Math.Abs(ahrs.EularAngles.Yaw) >> 8);
                    sendData[8] = Convert.ToByte((byte)Math.Abs(ahrs.EularAngles.Yaw));
                    udp.Send(sendData);

                    var ticks = (DateTime.Now - start).Ticks;
                    var counter = 1000d * 10000d / ticks;

                    // System.Console.WriteLine(counter);
                    if (counter < 500)
                    {
                        System.Console.WriteLine(counter);
                        indicator.Status = Tron.Device.Indicator.IndicatorStatus.WRINING;
                    }
                    else
                    {
                        indicator.Status = Tron.Device.Indicator.IndicatorStatus.RUNING;
                    }
                    start = DateTime.Now;
                });

            }
        }

    }
}
