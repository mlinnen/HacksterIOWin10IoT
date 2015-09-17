using System;
using System.Threading.Tasks;
using net.protosystem.GarageDoor;
using Windows.Devices.AllJoyn;
using Windows.Foundation;

namespace GarageDoor.Device
{
    class GarageDoorService : IGarageDoorService
    {
        string _location = "Unknown";
        readonly GarageDoorDriver _driver;

        public GarageDoorService(GarageDoorDriver driver)
        {
            _driver = driver;
        }

        public IAsyncOperation<GarageDoorGetServiceVersionResult> GetServiceVersionAsync(AllJoynMessageInfo info)
        {
            Task<GarageDoorGetServiceVersionResult> task = new Task<GarageDoorGetServiceVersionResult>(() =>
            {
                return GarageDoorGetServiceVersionResult.CreateSuccessResult(1);
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<GarageDoorGetVersionResult> GetVersionAsync(AllJoynMessageInfo info)
        {
            Task<GarageDoorGetVersionResult> task = new Task<GarageDoorGetVersionResult>(() =>
            {
                return GarageDoorGetVersionResult.CreateSuccessResult(1);
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<GarageDoorGetLocationResult> GetLocationAsync(AllJoynMessageInfo info)
        {
            Task<GarageDoorGetLocationResult> task = new Task<GarageDoorGetLocationResult>(() =>
            {
                return GarageDoorGetLocationResult.CreateSuccessResult(_location);
            });
            task.Start();
            return task.AsAsyncOperation();
        }
        public IAsyncOperation<GarageDoorSetLocationResult> SetLocationAsync(AllJoynMessageInfo info, string value)
        {
            Task<GarageDoorSetLocationResult> task = new Task<GarageDoorSetLocationResult>(() =>
            {
                _location = value;
                return GarageDoorSetLocationResult.CreateSuccessResult();
            });
            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<GarageDoorCloseGarageResult> CloseGarageAsync(AllJoynMessageInfo info)
        {
            Task<GarageDoorCloseGarageResult> task = new Task<GarageDoorCloseGarageResult>(() =>
            {
                try
                {
                    _driver.OpenGarageDoor(false);
                    return GarageDoorCloseGarageResult.CreateSuccessResult();
                }
                catch (Exception ex)
                {
                    return GarageDoorCloseGarageResult.CreateFailureResult(1);
                }
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<GarageDoorOpenGarageResult> OpenGarageAsync(AllJoynMessageInfo info)
        {
            Task<GarageDoorOpenGarageResult> task = new Task<GarageDoorOpenGarageResult>(() =>
            {
                _driver.OpenGarageDoor(true);
                return GarageDoorOpenGarageResult.CreateSuccessResult();
            });
            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<GarageDoorOpenOrCloseGarageResult> OpenOrCloseGarageAsync(AllJoynMessageInfo info, bool open)
        {
            Task<GarageDoorOpenOrCloseGarageResult> task = new Task<GarageDoorOpenOrCloseGarageResult>(() =>
            {
                _driver.OpenGarageDoor(open);
                return GarageDoorOpenOrCloseGarageResult.CreateSuccessResult();
            });
            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<GarageDoorGetDoorStateResult> GetDoorStateAsync(AllJoynMessageInfo info)
        {
            Task<GarageDoorGetDoorStateResult> task = new Task<GarageDoorGetDoorStateResult>(() =>
            {
                var newState = new GarageDoorDoorState();
                var status = _driver.GetCurrentStatus();
                newState.Value1 = Convert.ToByte(status);
                switch(status)
                {
                    case DoorStatus.Closed:
                        newState.Value2 = "Closed";
                        break;
                    case DoorStatus.Opened:
                        newState.Value2 = "Opened";
                        break;
                    case DoorStatus.Closing:
                        newState.Value2 = "Closing";
                        break;
                    case DoorStatus.Opening:
                        newState.Value2 = "Opening";
                        break;
                    default:
                        newState.Value2 = "Unknown";
                        break;
                }
                return GarageDoorGetDoorStateResult.CreateSuccessResult(newState);
            });
            task.Start();
            return task.AsAsyncOperation();
        }
    }
}
