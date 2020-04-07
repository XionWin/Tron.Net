using System;

namespace Tron.Device.Gyro
{
    public delegate EularAngles OnEularAnglesChangedHanlder();
    public delegate Quaternion OnQuaternionChangedHanlder();
    public interface IGyro
    {
        event OnEularAnglesChangedHanlder OnEularAnglesChanged;
        event OnQuaternionChangedHanlder OnQuaternionChanged;
    }
}
