using System;
using System.Threading.Tasks;
using net.protosystem.GarageDoor;
using Windows.Devices.AllJoyn;
using Windows.Foundation;

namespace GarageDoor
{
    class GarageDoorService : IGarageDoorService
    {
        string _location = "Unknown";
        string _currentState = "Unknown";

        public GarageDoorService()
        {

        }

        public IAsyncOperation<GarageDoorCloseGarageResult> CloseGarageAsync(AllJoynMessageInfo info)
        {
            Task<GarageDoorCloseGarageResult> task = new Task<GarageDoorCloseGarageResult>(() =>
            {
                try
                {
                    if (_currentState != "Closed")
                    {
                        _currentState = "Closing";
                    }
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

        public IAsyncOperation<GarageDoorGetCurrentStateResult> GetCurrentStateAsync(AllJoynMessageInfo info)
        {
            Task<GarageDoorGetCurrentStateResult> task = new Task<GarageDoorGetCurrentStateResult>(() =>
            {
                try
                {
                    return GarageDoorGetCurrentStateResult.CreateSuccessResult(_currentState);
                }
                catch (Exception ex)
                {
                    return GarageDoorGetCurrentStateResult.CreateFailureResult(1);
                }
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

        public IAsyncOperation<GarageDoorOpenGarageResult> OpenGarageAsync(AllJoynMessageInfo info)
        {
            Task<GarageDoorOpenGarageResult> task = new Task<GarageDoorOpenGarageResult>(() =>
            {
                if (_currentState != "Opened")
                {
                    _currentState = "Opening";
                }

                return GarageDoorOpenGarageResult.CreateSuccessResult();
            });
            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<GarageDoorOpenOrCloseGarageResult> OpenOrCloseGarageAsync(AllJoynMessageInfo info, bool open)
        {
            Task<GarageDoorOpenOrCloseGarageResult> task = new Task<GarageDoorOpenOrCloseGarageResult>(() =>
            {
                if (open)
                {
                    // TODO if the door is not already open then command it to open
                }
                else
                {
                    // TODO if the door is not already clode then command it to close

                }
                return GarageDoorOpenOrCloseGarageResult.CreateSuccessResult();
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
    }
}
