using System;

namespace Tron.Flight
{
    public class PIDController
    {
        private double _kp;
        private double _ki;
        private double _kd;
        private readonly double _maxErrorCumulative = 0;
        private double _previousErrorCumulative = 0d;
        private double _previousError = 0d;

        public PIDController(double proportionalGain, double integralGain, double derivativeGain)
        {
            this._kp = proportionalGain;
            this._ki = integralGain;
            this._kd = derivativeGain;
        }

        public double Manipulate(double setpoint, double processVariable, double elapsedTime)
        {
            double error = setpoint - processVariable;
            double outputProportionalGain = this._kp * error;

            // integral
            _previousErrorCumulative = Math.Min(Math.Max(_previousErrorCumulative + error * elapsedTime, -_maxErrorCumulative), _maxErrorCumulative);
            double outputIntegralGain = this._ki * _previousErrorCumulative;

            // derivative
            double futureError = (error - _previousError) / elapsedTime;
            double outputDerivativeGain = this._kd * futureError;
            _previousError = error;

            return Math.Max(Math.Min(outputProportionalGain + outputIntegralGain + outputDerivativeGain, 1), -1);
        }
    }
}
