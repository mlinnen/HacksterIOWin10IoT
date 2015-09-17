using net.protosystem.SmartSpaces.Environment.CurrentTemperature;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.UI.Core;

namespace GarageDoor.Consumer
{
    public class TemperatureControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CurrentTemperatureConsumer _consumer;
        private CurrentTemperatureWatcher _watcher;
        private int _temperature;
        private readonly CoreDispatcher dispatcher;

        public TemperatureControlViewModel()
        {
            this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }
        
        public int Temperature
        {
            get { return _temperature; }
            set
            {
                if (_temperature != value)
                {
                    _temperature = value;
                    NotifyPropertyChanged("Temperature");
                }
            }

        }
        public void Start()
        {
            AllJoynBusAttachment bus = new AllJoynBusAttachment();
            bus.AuthenticationMechanisms.Add(AllJoynAuthenticationMechanism.SrpAnonymous);

            _consumer = new CurrentTemperatureConsumer(bus);

            _watcher = new CurrentTemperatureWatcher(bus);
            _watcher.Added += _watcher_Added;
            _watcher.Start();
        }

        private async void _consumer_CurrentValueChanged(CurrentTemperatureConsumer sender, object args)
        {
            var result = await sender.GetCurrentValueAsync();
            // Temperature comes in as celsius so lets convert to fahrenheit
            double value = result.CurrentValue;
            Temperature = Convert.ToInt32(value * 9.0 / 5.0 + 32);
        }

        private async void _watcher_Added(CurrentTemperatureWatcher sender, AllJoynServiceInfo args)
        {
            CurrentTemperatureJoinSessionResult result = await CurrentTemperatureConsumer.JoinSessionAsync(args, sender);
            _consumer = result.Consumer;
            _consumer.CurrentValueChanged += _consumer_CurrentValueChanged;

        }


        private async void NotifyPropertyChanged(string propertyName)
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
