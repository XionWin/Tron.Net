using System;

namespace Tron.Flight
{
    public class AHRS
    {
        private Tron.Device.Gyro.MPU9250.Module _gyro;
        public AHRS(Tron.Device.Gyro.MPU9250.Module gyro, Action<AHRS> onReadCallback)
        {
            this._gyro = gyro;

            while (true)
            {
                this.EularAngles = this.read();
                onReadCallback(this);
            }
        }


        public Tron.Core.Data.Vector3 Accel
        {
            get;
            private set;
        }
        public Tron.Core.Data.Vector3 Gyro
        {
            get;
            private set;
        }
        public Tron.Core.Data.Vector3 Mag
        {
            get;
            private set;
        }


        public Core.Data.Quaternion Quaternion
        {
            get;
            private set;
        } = new Tron.Core.Data.Quaternion(1, 0, 0, 0);

        public Core.Data.EularAngles EularAngles
        {
            get;
            private set;
        }

        private Tron.Core.Data.EularAngles read()
        {
            var a = _gyro.Accel;
            var g = _gyro.Gyro;
            var m = _gyro.Mag;

            var ax = a.X * _gyro.Ares - _gyro.AccBias.X;
            var ay = a.Y * _gyro.Ares - _gyro.AccBias.Y;
            var az = a.Z * _gyro.Ares - _gyro.AccBias.Z;

            var gx = g.X * _gyro.Gres;
            var gy = g.Y * _gyro.Gres;
            var gz = g.Z * _gyro.Gres;

            var mx = (m.X * _gyro.Mres * _gyro.MagCalibration.X - _gyro.MagBias.X) * _gyro.MagScale.X;
            var my = (m.Y * _gyro.Mres * _gyro.MagCalibration.Y - _gyro.MagBias.Y) * _gyro.MagScale.Y;
            var mz = (m.Z * _gyro.Mres * _gyro.MagCalibration.Z - _gyro.MagBias.Z) * _gyro.MagScale.Z;

            this.Accel = new Tron.Core.Data.Vector3(
                ax,
                ay,
                az
            );
            this.Gyro = new Tron.Core.Data.Vector3(
                gx,
                gy,
                gz
            );
            this.Mag = new Tron.Core.Data.Vector3(
                mx,
                my,
                mz
            );

            // for (int i = 0; i < 10; i++)
            // {
            //     deltat = (DateTime.Now.Ticks - _lastUpdate) / 10000000.0f; // set integration time by time elapsed since last filter update
            //     _lastUpdate = DateTime.Now.Ticks;

            //     MadgwickQuaternionUpdate(-ax, +ay, +az, gx * Math.PI / 180.0f, -gy * Math.PI / 180.0f, -gz * Math.PI / 180.0f, my, -mx, mz);
            // }

            deltat = (DateTime.Now.Ticks - _lastUpdate) / 10000000.0f; // set integration time by time elapsed since last filter update
            _lastUpdate = DateTime.Now.Ticks;
            MadgwickQuaternionUpdate(-ax, +ay, +az, gx * Math.PI / 180.0f, -gy * Math.PI / 180.0f, -gz * Math.PI / 180.0f, my, -mx, mz);


            var a12 = 2.0f * (Quaternion.Q2 * Quaternion.Q3 + Quaternion.Q1 * Quaternion.Q4);
            var a22 = Quaternion.Q1 * Quaternion.Q1 + Quaternion.Q2 * Quaternion.Q2 - Quaternion.Q3 * Quaternion.Q3 - Quaternion.Q4 * Quaternion.Q4;
            var a31 = 2.0f * (Quaternion.Q1 * Quaternion.Q2 + Quaternion.Q3 * Quaternion.Q4);
            var a32 = 2.0f * (Quaternion.Q2 * Quaternion.Q4 - Quaternion.Q1 * Quaternion.Q3);
            var a33 = Quaternion.Q1 * Quaternion.Q1 - Quaternion.Q2 * Quaternion.Q2 - Quaternion.Q3 * Quaternion.Q3 + Quaternion.Q4 * Quaternion.Q4;
            var pitch = -Math.Asin(a32);
            var roll = Math.Atan2(a31, a33);
            var yaw = Math.Atan2(a12, a22);
            pitch *= 180.0f / Math.PI;
            yaw *= 180.0f / Math.PI;
            // yaw += 13.8f; // Declination at Danville, California is 13 degrees 48 minutes and 47 seconds on 2014-04-04
            // yaw += 3.89f;
            if (yaw < 0) yaw += 360.0f; // Ensure yaw stays between 0 and 360
            roll *= 180.0f / Math.PI;

            return new Tron.Core.Data.EularAngles(pitch, roll, yaw);
        }

        private const double GyroMeasError = Math.PI * (60.0 / 180.0);
        private static readonly double beta = Math.Sqrt(3.0 / 4.0) * GyroMeasError;
        private double deltat = 0;
        private double _lastUpdate = 0;
        private void MadgwickQuaternionUpdate(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz)
        {
            double q1 = Quaternion.Q1, q2 = Quaternion.Q2, q3 = Quaternion.Q3, q4 = Quaternion.Q4;
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

            this.Quaternion = new Core.Data.Quaternion(
                q1 * norm,
                q2 * norm,
                q3 * norm,
                q4 * norm
            );
        }
    }
}
