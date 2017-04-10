
namespace Dillon.Plugin.vJoy {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices.ComTypes;
    using Common;
    using PluginAPI;
    using PluginAPI.V1;
    using vJoyInterfaceWrap;

    public class vJoyMappingFactory
        : IMappingFactory {

        public vJoyMappingFactory(ILoggerAdapter logger) {
            _logger = logger;

            _logger.Debug("Attempting to initialise virtual joystick.");

            if (!_joy.vJoyEnabled()) {
                _logger.Warn("vJoy driver not enabled: Failed Getting vJoy attributes.");
                return;
            }

            _logger.Debug($"Vendor: {_joy.GetvJoyManufacturerString()}, Product :{_joy.GetvJoyProductString()}, Version Number:{_joy.GetvJoySerialNumberString()}");

            uint libVersion = 0, driverVersion = 0;
            if (!_joy.DriverMatch(ref libVersion, ref driverVersion)) {
                _logger.Warn($"Version of Driver ({driverVersion:X}) does NOT match DLL Version {libVersion:X}.");
                return;
            }

            _logger.Debug($"Running vJoy driver and library version {libVersion:X}.");

            if (SetDeviceId() == NoDeviceFound) {
                _logger.Warn("No vJoy device exists on this system.");
                return;
            }

            var deviceStatus = _joy.GetVJDStatus(_vDeviceId);
            switch (deviceStatus) {
                case VjdStat.VJD_STAT_OWN:
                    _logger.Debug($"Virtual device {_vDeviceId} is currently owned by this process, no need to reacquire.");
                    break;
                case VjdStat.VJD_STAT_FREE:
                    _logger.Debug($"Virtual device {_vDeviceId} is currently free, attempting to acquire.");
                    _deviceIsAcquired = _joy.AcquireVJD(_vDeviceId);
                    if (!_deviceIsAcquired) {
                        _logger.Warn($"Unable to acquire virtual device {_vDeviceId} even though it's status is VJD_STAT_FREE.");
                        return;
                    }
                    _logger.Debug($"Acquired device {_vDeviceId}.");
                    break;
                default:
                    _logger.Warn($"The status of virtual device {_vDeviceId} is currently {StatusString(deviceStatus)} ({StatusDescription(deviceStatus)}), Cannot continue.");
                    return;
            }

            _logger.Trace($"vJoy device acquired on process {_joy.GetOwnerPid(_vDeviceId)}");

            LogDeviceCapabilities();

            //// Acquire the target
            //if ((deviceStatus == VjdStat.VJD_STAT_OWN) || ((deviceStatus == VjdStat.VJD_STAT_FREE) && (! _joy.AcquireVJD(_vDeviceId))))
            //    Console.WriteLine($"Failed to acquire vJoy device number {_vDeviceId}.");
            //else
            //    Console.WriteLine($"Acquired: vJoy device number {_vDeviceId}.");

        }

        private void LogDeviceCapabilities() {
            //todo turn this into code which confirms caps
            var buttonCount = _joy.GetVJDButtonNumber(_vDeviceId);
            var hasXAxis = _joy.GetVJDAxisExist(_vDeviceId, HID_USAGES.HID_USAGE_X);
            var hasYAxis = _joy.GetVJDAxisExist(_vDeviceId, HID_USAGES.HID_USAGE_Y);
            var hasZAxis = _joy.GetVJDAxisExist(_vDeviceId, HID_USAGES.HID_USAGE_Z);
            _logger.Trace($"Device[{_vDeviceId}] capabilities: Buttons={buttonCount}; Has X axis: {hasXAxis}; Has Y axis: {hasYAxis}; Has Z axis: {hasZAxis}; ");
        }

        private string StatusString(VjdStat status) {
            if (status == VjdStat.VJD_STAT_BUSY)
                return "VJD_STAT_BUSY";
            if (status == VjdStat.VJD_STAT_MISS)
                return "VJD_STAT_MISS";
            return "Unspecified";
        }

        private string StatusDescription(VjdStat status) {
            if (status == VjdStat.VJD_STAT_BUSY)
                return "Device owned by another process";
            if (status == VjdStat.VJD_STAT_MISS)
                return "Device is not installed or disabled";
            return "General error";
        }

        private uint SetDeviceId() {
            for (var i = MinDeviceId; i <= MaxDeviceId; ++i) {
                if (!_joy.isVJDExists(i))
                    continue;
                _vDeviceId = i;
                break;
            }
            return _vDeviceId;
        }

        public void RegisterDependancy<T>(T dependancy) {
        }

        public IMapping Create(string name, IDictionary<string, object> map) {
            if (map == null)
                return new NullMapping();

            //todo pass in logger if needed in mappings
            foreach (var key in map.Keys) {
                switch (key) {
                    case "axis":
                        string axis = map[key].ToString();
                        if (axis.ToLower() == "x")
                            return new vJoyXAxisMapping(_vDeviceId, _joy);
                        if (axis.ToLower() == "y")
                            return new vJoyYAxisMapping(_vDeviceId, _joy);

                        return new vJoyDualAxisMapping(_vDeviceId, _joy);
                    case "buttons":
                        string buttons = map[key].ToString();
                        return new vJoyButtonMapping(GetButtons(buttons), _vDeviceId, _joy, _logger);
                }
            }

            return new NullMapping();
        }

        private uint[] GetButtons(string buttons) {
            return Array.ConvertAll(buttons.Split(','), Convert.ToUInt32);
        }

        ~vJoyMappingFactory() {
            if (!_deviceIsAcquired)
                return;

            var status = _joy.GetVJDStatus(_vDeviceId);
            if (status == VjdStat.VJD_STAT_OWN)
                _joy.RelinquishVJD(_vDeviceId);
        }

        private readonly vJoy _joy = new vJoy();
        private uint _vDeviceId = NoDeviceFound;
        private readonly ILoggerAdapter _logger;

        private const uint MinDeviceId = 1;
        private const uint MaxDeviceId = 16;
        private const int NoDeviceFound = 0;

        private bool _deviceIsAcquired = false;
    }

    }