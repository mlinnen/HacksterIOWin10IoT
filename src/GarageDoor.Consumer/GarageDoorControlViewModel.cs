using net.protosystem.GarageDoor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.UI.Core;

namespace GarageDoor.Consumer
{
    public class GarageDoorControlViewModel : INotifyPropertyChanged
    {
        private int _lastGarageDoorState;
        private int _garageDoorState;
        private string _garageDoorStateName;
        private double _openTime;
        private double _closeTime;
        private GarageDoorConsumer _consumer;
        private GarageDoorWatcher _watcher;

        public GarageDoorControlViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int GarageDoorState {
            get { return _garageDoorState; }
            set
            {
                if (_garageDoorState != value)
                {
                    _garageDoorState = value;
                    NotifyPropertyChanged("GarageDoorState");
                    switch(this.GarageDoorState)
                    {
                        case -1:
                            GarageDoorStateName = "";
                            break;
                        case 1:
                            GarageDoorStateName = "Opened";
                            break;
                        case 2:
                            GarageDoorStateName = "Closed";
                            break;
                        case 3:
                            GarageDoorStateName = "Opening";
                            break;
                        case 4:
                            GarageDoorStateName = "Closing";
                            break;
                        default:
                            GarageDoorStateName = "Unknown";
                            break;
                    }
                    NotifyPropertyChanged("GarageDoorStateName");
                }
            }

        }

        public double OpenTime
        {
            get { return _openTime; }
            set
            {
                if (_openTime != value)
                {
                    _openTime = value;
                    NotifyPropertyChanged("OpenTime");
                }
            }

        }

        public double CloseTime
        {
            get { return _closeTime; }
            set
            {
                if (_closeTime != value)
                {
                    _closeTime = value;
                    NotifyPropertyChanged("CloseTime");
                }
            }

        }

        public string GarageDoorStateName
        {
            get { return _garageDoorStateName; }
            set
            {
                if (_garageDoorStateName != value)
                {
                    _garageDoorStateName = value;
                    NotifyPropertyChanged("GarageDoorStateName");
                }
            }

        }

        public void Start()
        {
            AllJoynBusAttachment bus = new AllJoynBusAttachment();
            bus.AuthenticationMechanisms.Add(AllJoynAuthenticationMechanism.SrpAnonymous);

            _consumer = new GarageDoorConsumer(bus);
            
            _watcher = new GarageDoorWatcher(bus);
            _watcher.Added += _watcher_Added;
            _watcher.Start();
            GarageDoorState = -1;

        }

        private async void _watcher_Added(GarageDoorWatcher sender, AllJoynServiceInfo args)
        {
            GarageDoorJoinSessionResult result = await GarageDoorConsumer.JoinSessionAsync(args, sender);
            _consumer = result.Consumer;
            _consumer.Signals.GarageDoorStateChangedReceived += Signals_GarageDoorStateChangedReceived;
            var result2 = await _consumer.GetDoorStateAsync();
            GarageDoorState = Convert.ToInt32(result2.DoorState.Value1);

        }

        private void Signals_GarageDoorStateChangedReceived(GarageDoorSignals sender, GarageDoorGarageDoorStateChangedReceivedEventArgs args)
        {
            GarageDoorState = Convert.ToInt32(args.NewState);
            if (GarageDoorState== 1 && _lastGarageDoorState == 3)       // Current state is opened and last state was opening
                OpenTime = Math.Round((args.ElapsedTime / 1000.0), 1);
            else if (GarageDoorState == 2 && _lastGarageDoorState==4)   // Current state is closed and last state was closing
                CloseTime = Math.Round((args.ElapsedTime / 1000.0), 1);

            _lastGarageDoorState = GarageDoorState;

        }

        public async void Close()
        {
            await _consumer.CloseGarageAsync();
        }

        public async void Open()
        {
            await _consumer.OpenGarageAsync();
        }

        public async void NotifyPropertyChanged(string propertyName)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this,
                        new PropertyChangedEventArgs(propertyName));
                }
            });
        }

    }
}
