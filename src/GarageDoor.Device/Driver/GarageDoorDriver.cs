using net.protosystem.GarageDoor;
using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace GarageDoor.Device
{
    enum DoorStatus
    {
        Unknown=0,
        Opened = 1,
        Closed = 2,
        Opening=3,
        Closing=4,
    }
    class GarageDoorDriver
    {
        readonly GarageDoorProducer _producer;
        private const int DOOR_UP_PIN = 5;
        private const int DOOR_DOWN_PIN = 6;
        private const int DOOR_RELAY_PIN = 13;
        private GpioPin _doorUpPin;
        private GpioPin _doorDownPin;
        private GpioPin _doorRelayPin;
        DoorStatus _lastStatus = DoorStatus.Unknown;
        Stopwatch _stopWatch;
        private Timer _relayTimer;

        public GarageDoorDriver(GarageDoorProducer producer)
        {
            _producer = producer;
            InitGPIO();
            _relayTimer = new Timer(RelayTimeCallback, null, TimeSpan.FromMilliseconds(1500), TimeSpan.FromMilliseconds(0));

        }

        public DoorStatus GetCurrentStatus()
        {
            return _lastStatus;
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                return;
            }

            _doorUpPin = gpio.OpenPin(DOOR_UP_PIN);
            _doorDownPin = gpio.OpenPin(DOOR_DOWN_PIN);

            if (_doorUpPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                _doorUpPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                _doorUpPin.SetDriveMode(GpioPinDriveMode.Input);
            _doorUpPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _doorUpPin.ValueChanged += _doorUpPin_ValueChanged;

            if (_doorDownPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                _doorDownPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                _doorDownPin.SetDriveMode(GpioPinDriveMode.Input);
            _doorDownPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _doorDownPin.ValueChanged += _doorDownPin_ValueChanged;

            _doorRelayPin = gpio.OpenPin(DOOR_RELAY_PIN);
            _doorRelayPin.Write(GpioPinValue.High);
            _doorRelayPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void _doorDownPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            EvaluateDoorState();
        }

        private void _doorUpPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            EvaluateDoorState();
        }

        private void EvaluateDoorState()
        {
            var down = _doorDownPin.Read();
            var up = _doorUpPin.Read();
            var newStatus = DoorStatus.Unknown;
            if (up == GpioPinValue.High && down == GpioPinValue.Low)
                newStatus = DoorStatus.Closed;
            else if (up == GpioPinValue.Low && down == GpioPinValue.High)
                newStatus = DoorStatus.Opened;
            else if (up == GpioPinValue.High && down == GpioPinValue.High)
            {
                if (_lastStatus == DoorStatus.Closed)
                    newStatus = DoorStatus.Opening;
                else if (_lastStatus == DoorStatus.Opened)
                    newStatus = DoorStatus.Closing;
            }
            if (newStatus != _lastStatus)
            {
                var elapsedTime = 0;
                if (newStatus== DoorStatus.Opening || newStatus == DoorStatus.Closing)
                {
                    _stopWatch = Stopwatch.StartNew();
                }
                else if ((newStatus== DoorStatus.Closed || newStatus == DoorStatus.Opened) && _stopWatch!=null)
                {
                    _stopWatch.Stop();
                    elapsedTime = Convert.ToInt32(_stopWatch.ElapsedMilliseconds);
                }
                _producer.Signals.GarageDoorStateChanged(Convert.ToUInt32(_lastStatus), Convert.ToUInt32(newStatus), elapsedTime);
                _lastStatus = newStatus;
            }
        }

        public void Start()
        {

        }

        public void OpenGarageDoor(bool open)
        {
            if (open && (_lastStatus == DoorStatus.Closed || _lastStatus == DoorStatus.Unknown))
            {
                FireRelay();
            }
            else if (!open && (_lastStatus == DoorStatus.Opened || _lastStatus == DoorStatus.Unknown))
            {
                FireRelay();
            }
        }

        public void ToggleGarage()
        {
            FireRelay();
        }

        private void FireRelay()
        {
            _doorRelayPin.Write(GpioPinValue.Low);
            // Fire a timer that will open the relay after 1.5 seconds
            _relayTimer.Change(TimeSpan.FromMilliseconds(1500), TimeSpan.FromMilliseconds(0));
        }

        private void RelayTimeCallback(object state)
        {
            _doorRelayPin.Write(GpioPinValue.High);
        }
    }
}
