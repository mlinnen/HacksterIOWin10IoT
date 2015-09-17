using Windows.UI.Xaml.Controls;
using Windows.Devices.AllJoyn;
using net.protosystem.GarageDoor;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using System.Threading;
using System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GarageDoor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GarageDoorProducer _producer;
        private GarageDoorConsumer _consumer;
        private GarageDoorWatcher _watcher;
        private Timer _timer;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            
        }

        private void TimerCallBack(object state)
        {
            _producer.Signals.GarageDoorStateChanged(1, 2, "Single Garage", 10000);
            _timer.Change(10000, Timeout.Infinite);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            AllJoynBusAttachment bus = new AllJoynBusAttachment();
            bus.AuthenticationMechanisms.Add(AllJoynAuthenticationMechanism.SrpAnonymous);

            _producer = new GarageDoorProducer(bus);
            _producer.Service = new GarageDoorService();
            _producer.Start();
            _timer = new Timer(TimerCallBack, null, 10000, Timeout.Infinite);

            _watcher = new GarageDoorWatcher(bus);
            _watcher.Added += _watcher_Added;
            _watcher.Start();

            
        }

        private async void _watcher_Added(GarageDoorWatcher sender, AllJoynServiceInfo args)
        {
            GarageDoorJoinSessionResult result = await GarageDoorConsumer.JoinSessionAsync(args, sender);
            _consumer = result.Consumer;
            _consumer.Signals.GarageDoorStateChangedReceived += Signals_GarageDoorStateChangedReceived;
        }

        private void Signals_GarageDoorStateChangedReceived(GarageDoorSignals sender, GarageDoorGarageDoorStateChangedReceivedEventArgs args)
        {
            
        }
    }
}
