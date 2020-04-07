using System;
using System.Threading;

namespace Tron.Device.Indicator
{
    public enum IndicatorStatus : byte
    {
        NULL,
        INIT,
        STANDBY,
        RUNING,
        WRINING,
        ERROR,
    }

    internal struct IndicatorTask
    {
        private bool _flag;
        public IndicatorTask(Action action, Action circleAction = null)
        {
            this.Action = action;
            this.CircleAction = circleAction;
            this._flag = true;
        }
        public Action Action
        {
            get;
            private set;
        }
        public Action CircleAction
        {
            get;
            private set;
        }

        public void Run()
        {
            if(this._flag)
            {
                this.Action?.Invoke();
                this._flag = false;
            }
            else
            {
                this.CircleAction?.Invoke();
            }
        }
    }

    public class IntegratedIndicator : IDisposable
    {
        private const int _CIRCLE_TOTAL = 1000;
        private const int _SHORT_ON_DELAY = 50;
        private const int _SHORT_OFF_DELAY = 100;
        private const int _SHORT_TOTAL_DELAY = _SHORT_ON_DELAY + _SHORT_OFF_DELAY;
        private const int _LONG_ON_DELAY = 250;
        private const int _LONG_OFF_DELAY = 100;

        private bool _switch = true;
        public IntegratedIndicator()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                IndicatorStatus lastStatus = this.Status;
                IndicatorTask lastTask = new IndicatorTask(null);
                while (_switch)
                {
                    if (this.Status != lastStatus)
                    {
                        var task = CreateTaskFromStatus(this.Status);
                        task.Run();
                        lastStatus = this.Status;
                        lastTask = task;
                    }
                    else if(lastTask.CircleAction != null)
                    {
                        lastTask.Run();
                    }
                    else
                    {
                        Hardware.Library.Delay(_SHORT_TOTAL_DELAY);
                    }
                }
            }));
        }

        private LEDIndicator _led = LEDIndicator.Instance;
        private BuzzerIndicator _buzzer = BuzzerIndicator.Instance;
        public IndicatorStatus Status
        {
            get;
            set;
        } = IndicatorStatus.NULL;

        private IndicatorTask CreateTaskFromStatus(IndicatorStatus status)
        {
            switch (status)
            {
                case IndicatorStatus.NULL:
                    return new IndicatorTask(() =>
                    {
                        this.Reset();
                    });
                case IndicatorStatus.INIT:
                    return new IndicatorTask(() =>
                    {
                        this.Reset();
                        this._led.Color = IndicatorColor.BLUE;
                        this._buzzer.Enable = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Enable = false;
                        Hardware.Library.Delay(_SHORT_OFF_DELAY);
                    });
                case IndicatorStatus.STANDBY:
                    return new IndicatorTask(() =>
                    {
                        this.Reset();
                        this._led.Color = IndicatorColor.WHITE;
                        this._buzzer.Enable = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._buzzer.Enable = false;
                        Hardware.Library.Delay(_SHORT_OFF_DELAY);
                        this._buzzer.Enable = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Enable = false;
                        Hardware.Library.Delay(_SHORT_OFF_DELAY);
                    });
                case IndicatorStatus.RUNING:
                    return new IndicatorTask(() =>
                    {
                        this.Reset();
                        this._led.Color = IndicatorColor.GREEN;
                        this._buzzer.Enable = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Enable = false;
                        Hardware.Library.Delay(_CIRCLE_TOTAL - _SHORT_ON_DELAY);
                    },
                    () =>
                    {
                        this._led.Color = IndicatorColor.GREEN;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        Hardware.Library.Delay(_CIRCLE_TOTAL - _SHORT_ON_DELAY); 
                    });
                case IndicatorStatus.WRINING:
                    return new IndicatorTask(() =>
                    {
                        this.Reset();
                        this._led.Color = IndicatorColor.YELLOW;
                        this._buzzer.Enable = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Enable = false;
                        Hardware.Library.Delay(_CIRCLE_TOTAL - _SHORT_ON_DELAY);
                    },
                    () =>
                    {
                        this._led.Color = IndicatorColor.YELLOW;
                        this._buzzer.Enable = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Enable = false;
                        Hardware.Library.Delay(_CIRCLE_TOTAL - _SHORT_ON_DELAY);
                    });
                case IndicatorStatus.ERROR:
                    return new IndicatorTask(() =>
                    {
                        this.Reset();
                        this._led.Color = IndicatorColor.RED;
                        this._buzzer.Enable = true;
                        Hardware.Library.Delay(_LONG_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Enable = false;
                        Hardware.Library.Delay(_LONG_OFF_DELAY);
                    },
                    () =>
                    {
                        this._led.Color = IndicatorColor.RED;
                        this._buzzer.Enable = true;
                        Hardware.Library.Delay(_LONG_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Enable = false;
                        Hardware.Library.Delay(_LONG_OFF_DELAY);
                    });
                default:
                    return new IndicatorTask(null);
            }
        }
        
        private void Reset()
        {
            this._led.Reset();
            this._buzzer.Reset();
        }

        public void Dispose()
        {
            this._switch = false;
            Hardware.Library.Delay(_CIRCLE_TOTAL);
            this.Reset();
        }
    }
}
