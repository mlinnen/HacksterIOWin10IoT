using System;
using System.Threading.Tasks;
using net.protosystem.SmartSpaces.Environment.CurrentTemperature;
using Windows.Devices.AllJoyn;
using Windows.Foundation;
using System.Threading;

namespace GarageDoor.Device
{
    public class CurrentTemperatureService : ICurrentTemperatureService
    {
        private string _location="Single Garage";
        private const int _version = 1;
        private readonly IDoubleSensorDriver _tempSensorDriver;
        private double _temperature;
        private Timer _timer;
        private const int _sampleRateMilliseconds = 1000;

        public CurrentTemperatureService(IDoubleSensorDriver tempSensorDriver)
        {
            _tempSensorDriver = tempSensorDriver;
            _timer = new Timer(tcb, null, TimeSpan.FromMilliseconds(_sampleRateMilliseconds), TimeSpan.FromMilliseconds(_sampleRateMilliseconds));
        }

        private async void tcb(object state)
        {
            
            _temperature = await _tempSensorDriver.Read();
        }

        public IAsyncOperation<CurrentTemperatureGetCurrentValueResult> GetCurrentValueAsync(AllJoynMessageInfo info)
        {
            Task<CurrentTemperatureGetCurrentValueResult> task = new Task<CurrentTemperatureGetCurrentValueResult>(() =>
            {
                return CurrentTemperatureGetCurrentValueResult.CreateSuccessResult(_temperature); 
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<CurrentTemperatureGetLocationResult> GetLocationAsync(AllJoynMessageInfo info)
        {
            Task<CurrentTemperatureGetLocationResult> task = new Task<CurrentTemperatureGetLocationResult>(() =>
            {
                return CurrentTemperatureGetLocationResult.CreateSuccessResult(_location);
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<CurrentTemperatureGetVersionResult> GetVersionAsync(AllJoynMessageInfo info)
        {
            Task<CurrentTemperatureGetVersionResult> task = new Task<CurrentTemperatureGetVersionResult>(() =>
            {
                return CurrentTemperatureGetVersionResult.CreateSuccessResult(_version);
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<CurrentTemperatureSetLocationResult> SetLocationAsync(AllJoynMessageInfo info, string value)
        {
            Task<CurrentTemperatureSetLocationResult> task = new Task<CurrentTemperatureSetLocationResult>(() =>
            {
                _location = value;
                return CurrentTemperatureSetLocationResult.CreateSuccessResult();
            });
            task.Start();
            return task.AsAsyncOperation();
        }
    }
}
