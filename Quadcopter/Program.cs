#define ENABLE_LED
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
                double minCounter = double.MaxValue;
                Tron.Core.Data.EularAngles eular = new Tron.Core.Data.EularAngles();
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
                        eular = read(gyro);
                        Tron.Linux.System.Sleep(50);
                    }

                    minCounter = Math.Min(target / (DateTime.Now - start).TotalSeconds, minCounter);
                    if (minCounter < 2000)
                    {
                        indicator.Status = Tron.Device.Indicator.IndicatorStatus.WRINING;
                    }
                    else
                    {
                        indicator.Status = Tron.Device.Indicator.IndicatorStatus.RUNING;
                    }
                    if ((DateTime.Now - lastUpdate).TotalMilliseconds > 10)
                    {
                        System.Console.Write
                        (
                            "ax: {0} ay: {1} az: {2}\t",
                            _Accel.X.ToString("N2").PadLeft(8, ' '),
                            _Accel.Y.ToString("N2").PadLeft(8, ' '),
                            _Accel.Z.ToString("N2").PadLeft(8, ' ')
                        );

                        System.Console.Write
                        (
                            "gx: {0} gy: {1} gz: {2}\t",
                            _Gyro.X.ToString("N2").PadLeft(8, ' '),
                            _Gyro.Y.ToString("N2").PadLeft(8, ' '),
                            _Gyro.Z.ToString("N2").PadLeft(8, ' ')
                        );

                        System.Console.Write
                        (
                            "mx: {0} my: {1} mz: {2}\t",
                            _Mag.X.ToString("N2").PadLeft(8, ' '),
                            _Mag.Y.ToString("N2").PadLeft(8, ' '),
                            _Mag.Z.ToString("N2").PadLeft(8, ' ')
                        );

                        System.Console.Write
                        (
                            "PITCH: {0} ROLL: {1} YAW: {2}\t",
                            eular.Pitch.ToString("N2").PadLeft(7, ' '),
                            eular.Roll.ToString("N2").PadLeft(7, ' '),
                            eular.Yaw.ToString("N2").PadLeft(7, ' ')
                        );


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

        private static Tron.Core.Data.Vector3 _Accel = new Tron.Core.Data.Vector3();
        private static Tron.Core.Data.Vector3 _Gyro = new Tron.Core.Data.Vector3();
        private static Tron.Core.Data.Vector3 _Mag = new Tron.Core.Data.Vector3();
        private static Tron.Core.Data.EularAngles read(Tron.Device.Gyro.MPU9250.Module gyro)
        {

            var a = gyro.Accel;
            var g = gyro.Gyro;
            var m = gyro.Mag;

            var ax = a.X * gyro.Ares - gyro.AccBias.X;
            var ay = a.Y * gyro.Ares - gyro.AccBias.Y;
            var az = a.Z * gyro.Ares - gyro.AccBias.Z;

            var gx = g.X * gyro.Gres;
            var gy = g.Y * gyro.Gres;
            var gz = g.Z * gyro.Gres;

            var mx = (m.X * gyro.Mres * gyro.MagCalibration.X - gyro.MagBias.X) * gyro.MagScale.X;
            var my = (m.Y * gyro.Mres * gyro.MagCalibration.Y - gyro.MagBias.Y) * gyro.MagScale.Y;
            var mz = (m.Z * gyro.Mres * gyro.MagCalibration.Z - gyro.MagBias.Z) * gyro.MagScale.Z;


            _Accel = new Tron.Core.Data.Vector3(
                ax,
                ay,
                az
            );
            _Gyro = new Tron.Core.Data.Vector3(
                gx,
                gy,
                gz
            );
            _Mag = new Tron.Core.Data.Vector3(
                mx,
                my,
                mz
            );


            // for (var i = 0; i < 10; i++)
            // { // iterate a fixed number of times per data read cycle
            //     deltat = (DateTime.Now.Ticks - lastUpdate) / 100000.0f; // set integration time by time elapsed since last filter update
            //     lastUpdate = DateTime.Now.Ticks;

            //     MadgwickQuaternionUpdate(-ax, +ay, +az, gx * Math.PI / 180.0f, -gy * Math.PI / 180.0f, -gz * Math.PI / 180.0f, my, -mx, mz);
            // }

            for (int i = 0; i < 10; i++)
            {
                deltat = (DateTime.Now.Ticks - lastUpdate) / 1000000.0f; // set integration time by time elapsed since last filter update
                lastUpdate = DateTime.Now.Ticks;

                MadgwickQuaternionUpdate(-ax, +ay, +az, gx * Math.PI / 180.0f, -gy * Math.PI / 180.0f, -gz * Math.PI / 180.0f, my, -mx, mz);
            }


            var a12 = 2.0f * (q.Q2 * q.Q3 + q.Q1 * q.Q4);
            var a22 = q.Q1 * q.Q1 + q.Q2 * q.Q2 - q.Q3 * q.Q3 - q.Q4 * q.Q4;
            var a31 = 2.0f * (q.Q1 * q.Q2 + q.Q3 * q.Q4);
            var a32 = 2.0f * (q.Q2 * q.Q4 - q.Q1 * q.Q3);
            var a33 = q.Q1 * q.Q1 - q.Q2 * q.Q2 - q.Q3 * q.Q3 + q.Q4 * q.Q4;
            var pitch = -Math.Asin(a32);
            var roll = Math.Atan2(a31, a33);
            var yaw = Math.Atan2(a12, a22);
            pitch *= 180.0f / Math.PI;
            yaw *= 180.0f / Math.PI;
            yaw += 13.8f; // Declination at Danville, California is 13 degrees 48 minutes and 47 seconds on 2014-04-04
            if (yaw < 0) yaw += 360.0f; // Ensure yaw stays between 0 and 360
            roll *= 180.0f / Math.PI;

            return new Tron.Core.Data.EularAngles(pitch, roll, yaw);
        }

        private static Tron.Core.Data.Quaternion q = new Tron.Core.Data.Quaternion(1, 0, 0, 0);
        private const double GyroMeasError = Math.PI * (60.0 / 180.0);
        private static readonly double beta = Math.Sqrt(3.0 / 4.0) * GyroMeasError;
        private static double deltat = 0;
        private static double lastUpdate = 0;
        private static void MadgwickQuaternionUpdate(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz)
        {
            double q1 = q.Q1, q2 = q.Q2, q3 = q.Q3, q4 = q.Q4;
            double norm;
            double hx, hy, _2bx, _2bz;
            double s1, s2, s3, s4;
            double qDot1, qDot2, qDot3, qDot4;

            // Auxiliary variables to avoid repeated arithmetic
            double _2q1mx;
            double _2q1my;
            double _2q1mz;
            double _2q2mx;
            double _4bx;
            double _4bz;
            double _2q1 = 2.0f * q1;
            double _2q2 = 2.0f * q2;
            double _2q3 = 2.0f * q3;
            double _2q4 = 2.0f * q4;
            double _2q1q3 = 2.0f * q1 * q3;
            double _2q3q4 = 2.0f * q3 * q4;
            double q1q1 = q1 * q1;
            double q1q2 = q1 * q2;
            double q1q3 = q1 * q3;
            double q1q4 = q1 * q4;
            double q2q2 = q2 * q2;
            double q2q3 = q2 * q3;
            double q2q4 = q2 * q4;
            double q3q3 = q3 * q3;
            double q3q4 = q3 * q4;
            double q4q4 = q4 * q4;

            // Normalise accelerometer measurement
            norm = Math.Sqrt(ax * ax + ay * ay + az * az);
            if (norm == 0.0f) return; // handle NaN
            norm = 1.0f / norm;
            ax *= norm;
            ay *= norm;
            az *= norm;

            // Normalise magnetometer measurement
            norm = Math.Sqrt(mx * mx + my * my + mz * mz);
            if (norm == 0.0f) return; // handle NaN
            norm = 1.0f / norm;
            mx *= norm;
            my *= norm;
            mz *= norm;

            // Normalise accelerometer measurement
            norm = Math.Sqrt(ax * ax + ay * ay + az * az);
            if (norm == 0.0f) return; // handle NaN
            norm = 1.0f / norm;
            ax *= norm;
            ay *= norm;
            az *= norm;

            // Normalise magnetometer measurement
            norm = Math.Sqrt(mx * mx + my * my + mz * mz);
            if (norm == 0.0f) return; // handle NaN
            norm = 1.0f / norm;
            mx *= norm;
            my *= norm;
            mz *= norm;

            // Reference direction of Earth's magnetic field
            _2q1mx = 2.0f * q1 * mx;
            _2q1my = 2.0f * q1 * my;
            _2q1mz = 2.0f * q1 * mz;
            _2q2mx = 2.0f * q2 * mx;
            hx = mx * q1q1 - _2q1my * q4 + _2q1mz * q3 + mx * q2q2 + _2q2 * my * q3 + _2q2 * mz * q4 - mx * q3q3 - mx * q4q4;
            hy = _2q1mx * q4 + my * q1q1 - _2q1mz * q2 + _2q2mx * q3 - my * q2q2 + my * q3q3 + _2q3 * mz * q4 - my * q4q4;
            _2bx = Math.Sqrt(hx * hx + hy * hy);
            _2bz = -_2q1mx * q3 + _2q1my * q2 + mz * q1q1 + _2q2mx * q4 - mz * q2q2 + _2q3 * my * q4 - mz * q3q3 + mz * q4q4;
            _4bx = 2.0f * _2bx;
            _4bz = 2.0f * _2bz;

            // Gradient decent algorithm corrective step
            s1 = -_2q3 * (2.0f * q2q4 - _2q1q3 - ax) + _2q2 * (2.0f * q1q2 + _2q3q4 - ay) - _2bz * q3 * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (-_2bx * q4 + _2bz * q2) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + _2bx * q3 * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
            s2 = _2q4 * (2.0f * q2q4 - _2q1q3 - ax) + _2q1 * (2.0f * q1q2 + _2q3q4 - ay) - 4.0f * q2 * (1.0f - 2.0f * q2q2 - 2.0f * q3q3 - az) + _2bz * q4 * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (_2bx * q3 + _2bz * q1) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + (_2bx * q4 - _4bz * q2) * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
            s3 = -_2q1 * (2.0f * q2q4 - _2q1q3 - ax) + _2q4 * (2.0f * q1q2 + _2q3q4 - ay) - 4.0f * q3 * (1.0f - 2.0f * q2q2 - 2.0f * q3q3 - az) + (-_4bx * q3 - _2bz * q1) * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (_2bx * q2 + _2bz * q4) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + (_2bx * q1 - _4bz * q3) * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
            s4 = _2q2 * (2.0f * q2q4 - _2q1q3 - ax) + _2q3 * (2.0f * q1q2 + _2q3q4 - ay) + (-_4bx * q4 + _2bz * q2) * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (-_2bx * q1 + _2bz * q3) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + _2bx * q2 * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
            norm = Math.Sqrt(s1 * s1 + s2 * s2 + s3 * s3 + s4 * s4);    // normalise step magnitude
            norm = 1.0f / norm;
            s1 *= norm;
            s2 *= norm;
            s3 *= norm;
            s4 *= norm;

            // Compute rate of change of quaternion
            qDot1 = 0.5f * (-q2 * gx - q3 * gy - q4 * gz) - beta * s1;
            qDot2 = 0.5f * (q1 * gx + q3 * gz - q4 * gy) - beta * s2;
            qDot3 = 0.5f * (q1 * gy - q2 * gz + q4 * gx) - beta * s3;
            qDot4 = 0.5f * (q1 * gz + q2 * gy - q3 * gx) - beta * s4;

            // Integrate to yield quaternion
            q1 += qDot1 * deltat;
            q2 += qDot2 * deltat;
            q3 += qDot3 * deltat;
            q4 += qDot4 * deltat;
            norm = Math.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4);    // normalise quaternion
            norm = 1.0f / norm;
            q.Q1 = q1 * norm;
            q.Q2 = q2 * norm;
            q.Q3 = q3 * norm;
            q.Q4 = q4 * norm;
        }
    }
}
