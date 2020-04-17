using System;
using System.Collections.Generic;
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
            if (this._flag)
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

    public class Module : IDisposable
    {
        private const int _CIRCLE_TOTAL = 1000;
        private const int _SHORT_ON_DELAY = 50;
        private const int _SHORT_OFF_DELAY = 100;
        private const int _SHORT_TOTAL_DELAY = _SHORT_ON_DELAY + _SHORT_OFF_DELAY;
        private const int _LONG_ON_DELAY = 250;
        private const int _LONG_OFF_DELAY = 100;

        private Queue<IndicatorStatus> _status_queue = new Queue<IndicatorStatus>();

        private bool _switch = true;
        public Module(bool enableLED = true, bool enableBuzzer = true)
        {
            this.LEDSwitch = enableLED;
            this.BuzzerSwitch = enableBuzzer;
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                IndicatorStatus lastStatus = this.Status;
                IndicatorTask lastTask = new IndicatorTask(null);
                while (_switch)
                {
                    while (this._status_queue.Count > 1 && this._status_queue.Peek() == lastStatus)
                    {
                        this._status_queue.Dequeue();
                    }

                    if (this.Status != lastStatus)
                    {
                        var task = CreateTaskFromStatus(this.Status);
                        task.Run();
                        lastStatus = this.Status;
                        lastTask = task;
                    }
                    else if (lastTask.CircleAction != null)
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

        public bool LEDSwitch
        {
            get => this._led.Enable;
            set => this._led.Enable = value;
        }
        public bool BuzzerSwitch
        {
            get => this._buzzer.Enable;
            set => this._buzzer.Enable = value;
        }

        private LEDIndicator _led = LEDIndicator.Instance;

        private BuzzerIndicator _buzzer = BuzzerIndicator.Instance;

        private IndicatorStatus _last_added = IndicatorStatus.NULL;
        public IndicatorStatus Status
        {
            get => this._status_queue.Count > 0 ? this._status_queue.Peek() : IndicatorStatus.NULL;
            set
            {
                if (this._status_queue.Count < 100 && _last_added != value)
                {
                    this._status_queue.Enqueue(value);
                    this._last_added = value;
                }
            }
        }

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
                        this._buzzer.Value = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Value = false;
                        Hardware.Library.Delay(_SHORT_OFF_DELAY);
                    });
                case IndicatorStatus.STANDBY:
                    return new IndicatorTask(() =>
                    {
                        this.Reset();
                        this._led.Color = IndicatorColor.WHITE;
                        this._buzzer.Value = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._buzzer.Value = false;
                        Hardware.Library.Delay(_SHORT_OFF_DELAY);
                        this._buzzer.Value = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Value = false;
                        Hardware.Library.Delay(_SHORT_OFF_DELAY);
                    });
                case IndicatorStatus.RUNING:
                    return new IndicatorTask(() =>
                    {
                        this.Reset();
                        this._led.Color = IndicatorColor.GREEN;
                        this._buzzer.Value = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Value = false;
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
                        this._buzzer.Value = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Value = false;
                        Hardware.Library.Delay(_CIRCLE_TOTAL - _SHORT_ON_DELAY);
                    },
                    () =>
                    {
                        this._led.Color = IndicatorColor.YELLOW;
                        this._buzzer.Value = true;
                        Hardware.Library.Delay(_SHORT_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Value = false;
                        Hardware.Library.Delay(_CIRCLE_TOTAL - _SHORT_ON_DELAY);
                    });
                case IndicatorStatus.ERROR:
                    return new IndicatorTask(() =>
                    {
                        this.Reset();
                        this._led.Color = IndicatorColor.RED;
                        this._buzzer.Value = true;
                        Hardware.Library.Delay(_LONG_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Value = false;
                        Hardware.Library.Delay(_LONG_OFF_DELAY);
                    },
                    () =>
                    {
                        this._led.Color = IndicatorColor.RED;
                        this._buzzer.Value = true;
                        Hardware.Library.Delay(_LONG_ON_DELAY);
                        this._led.Color = IndicatorColor.NULL;
                        this._buzzer.Value = false;
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
