using System;
using Windows.Devices.AllJoyn;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using net.protosystem.GarageDoor;
using net.protosystem.SmartSpaces.Environment.CurrentTemperature;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GarageDoor.Device
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool _firstTime=true;
        private GarageDoorProducer _garageDoorProducer;
        private GarageDoorDriver _garageDoorDriver;
        private CurrentTemperatureProducer _garageTempProducer;
        private TemperatureDriver _tempHumidityDriver;
        private DispatcherTimer timer;
        private Mcp3008 _adcDriver;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_adcDriver != null)
                _adcDriver.Dispose();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            AllJoynBusAttachment bus = new AllJoynBusAttachment();
            bus.AuthenticationMechanisms.Add(AllJoynAuthenticationMechanism.SrpAnonymous);

            _garageDoorProducer = new GarageDoorProducer(bus);
            _garageDoorDriver = new GarageDoorDriver(_garageDoorProducer);
            _garageDoorProducer.Service = new GarageDoorService(_garageDoorDriver);
            _garageDoorProducer.Start();

            AllJoynBusAttachment bus2 = new AllJoynBusAttachment();
            bus2.AuthenticationMechanisms.Add(AllJoynAuthenticationMechanism.SrpAnonymous);

            _adcDriver = new Mcp3008(0);
            await _adcDriver.Connect();

            _garageTempProducer = new CurrentTemperatureProducer(bus2);
            _garageTempProducer.Service = new CurrentTemperatureService(new TemperatureDriver(_adcDriver, 0,_garageTempProducer));
            _garageTempProducer.Start();

        }

        private async void Timer_Tick(object sender, object e)
        {
            this.timer.Stop();
            if (_firstTime)
            {
                _adcDriver = new Mcp3008(0);
                await _adcDriver.Connect();
                //int data = _adcDriver.Read(0);
                _firstTime = false;
            }

            int value = _adcDriver.Read(0);
            rawValue.Text = value.ToString();
            double voltage = (value / 1024.0 * 5.16);
            //double voltage = (value - 0.5)* 100;
            voltageValue.Text = voltage.ToString();
            //double degrees = 100 * (voltage) - 50;
            double degrees = (voltage - 0.5) * 100;
            tempValue.Text = degrees.ToString();
            double degreesFahrenheit = (degrees * 9.0 / 5.16) + 32;
            tempFarhrenheitValue.Text = degreesFahrenheit.ToString();
            this.timer.Start();
        }

        private async void ReadData()
        {

        }
    }
}
