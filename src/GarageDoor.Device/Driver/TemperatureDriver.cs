using System;
using net.protosystem.SmartSpaces.Environment.CurrentTemperature;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using System.Threading.Tasks;

namespace GarageDoor.Device
{
    class TemperatureDriver: IDoubleSensorDriver
    {
        private readonly Mcp3008 _spiDriver;
        private double _lastDegrees = 0.0;
        int _port; // 0 thru 7
        private readonly CurrentTemperatureProducer _producer;
        private double[] _sensorReadings = new double[10];
        private int _index=0;
        private int _totalReadings=0;
        private double _total;

        public TemperatureDriver(Mcp3008 spiDriver, int port, CurrentTemperatureProducer producer)
        {
            if (port < 0 || port > 7)
                throw new ArgumentException("The port must be 0 - 7");
            _spiDriver = spiDriver;
            _producer = producer;
            _port = port;
        }

        public async Task<double> Read()
        {
            double value = 0.0;
            if (!_spiDriver.Connected)
                await _spiDriver.Connect();

            value = _spiDriver.Read(_port);
            // Convert the raw value into a voltage
            double voltage = (value / 1024.0 * 5.16);
            // Convert the volteage into degrees celsius
            double degrees = (voltage - 0.5) * 100;

            degrees = Smooth(degrees);
            degrees = Math.Round(degrees, 1, MidpointRounding.AwayFromZero);

            // If the degrees have changed since the last time we read it then notify all consumers of the service
            if (degrees != _lastDegrees)
            {
                _lastDegrees = degrees;
                _producer.EmitCurrentValueChanged();
            }
            return degrees;
        }

        private double Smooth(double newValue)
        {
            double returnValue = 0;
            _total = _total - _sensorReadings[_index];
            _sensorReadings[_index] = newValue;
            _total = _total + _sensorReadings[_index];
            _index++;
            _totalReadings++;
            if (_index >= 10)
                _index = 0;
            if (_totalReadings >= 10)
                _totalReadings = 10;
            returnValue = _total / _totalReadings;
            return returnValue;
        }
    }
}
