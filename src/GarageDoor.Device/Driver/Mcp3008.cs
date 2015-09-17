using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace GarageDoor.Device
{
    public class Mcp3008 : IDisposable
    {
        private SpiConnectionSettings _settings = null;
        private SpiDevice _device = null;

        public Mcp3008(int chipSelectLine)
        {
            this.Settings = new SpiConnectionSettings(chipSelectLine);

            this.Settings.ClockFrequency = 500000;
            this.Settings.Mode = SpiMode.Mode0;
            this.Settings.SharingMode = SpiSharingMode.Exclusive;
        }

        public SpiConnectionSettings Settings
        {
            get
            {
                return _settings;
            }
            private set
            {
                this._settings = value;
            }
        }

        public bool Connected { get { return _device!=null; } }

        public async Task Connect()
        {
            if (_device == null)
            {
                string selector = SpiDevice.GetDeviceSelector(string.Format("SPI{0}",Settings.ChipSelectLine));
                var deviceInfo = await DeviceInformation.FindAllAsync(selector);
                if (deviceInfo.Count == 0)
                    return;
                _device = await SpiDevice.FromIdAsync(deviceInfo[0].Id, this.Settings);
            }
        }

        public SpiDevice Device
        {
            get
            {
                if (_device == null)
                {
                    throw new Exception("Not initialized");
                }

                return _device;
            }
            private set
            {
                this._device = value;
            }
        }

        public int Read(int port)
        {
            int returnValue = 0;
            if (_device != null)
            {
                byte[] readBuffer = new byte[3];

                byte[] writeBuffer = new byte[3] { (byte)1, (byte)(port + 8 << 4), 0x00 };

                _device.TransferFullDuplex(writeBuffer, readBuffer);

                returnValue = ((readBuffer[1] & 3) << 8) + readBuffer[2];
            }
            else
            {
                throw new Exception("Not initialized");
            }

            return returnValue;

        }

        public void Dispose()
        {
            if (_device!=null)
            {
                _device.Dispose();
                _device = null;
            }
        }
    }
}
