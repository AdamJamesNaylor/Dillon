namespace Dillon.Plugin.vJoy {
    using System;
    using Common;
    using PluginAPI.V1;
    using vJoyInterfaceWrap;

    public class vJoyXAxisMapping
        : vJoyAxisMappingBase {

        public vJoyXAxisMapping(uint vDeviceId, vJoy joy)
            : base(vDeviceId, joy) { }

        public override void Execute(Update update) {
            if (Joy.GetVJDStatus(DeviceId) != VjdStat.VJD_STAT_OWN) 
                Joy.AcquireVJD(DeviceId);

            Joy.SetAxis((int)(AxisRanges[0].Max * update.NormalisedX), DeviceId, HID_USAGES.HID_USAGE_X);
        }
    }

    public class vJoyYAxisMapping
        : vJoyAxisMappingBase {

        public vJoyYAxisMapping(uint vDeviceId, vJoy joy)
            : base(vDeviceId, joy) { }

        public override void Execute(Update update) {
            if (Joy.GetVJDStatus(DeviceId) != VjdStat.VJD_STAT_OWN) {
                Joy.AcquireVJD(DeviceId);
            }

            Joy.SetAxis((int)(AxisRanges[1].Max * update.NormalisedY), DeviceId, HID_USAGES.HID_USAGE_Y);
        }
    }

    public class vJoyDualAxisMapping
        : vJoyAxisMappingBase {

        public vJoyDualAxisMapping(uint vDeviceId, vJoy joy)
            : base(vDeviceId, joy) { }

        public override void Execute(Update update) {
            if (Joy.GetVJDStatus(DeviceId) != VjdStat.VJD_STAT_OWN)
                Joy.AcquireVJD(DeviceId);

            Joy.SetAxis((int)(AxisRanges[0].Max * update.NormalisedX), DeviceId, HID_USAGES.HID_USAGE_X);
            Joy.SetAxis((int)(AxisRanges[1].Max * update.NormalisedY), DeviceId, HID_USAGES.HID_USAGE_Y);
        }
    }

    public abstract class vJoyAxisMappingBase
        : IMapping {

        protected struct AxisRange {
            public long Min;
            public long Max;
            public long Dead;
        }

        protected vJoyAxisMappingBase(uint deviceId, vJoy joy) {
            DeviceId = deviceId;
            Joy = joy;

            AxisRanges[0] = GetRange(HID_USAGES.HID_USAGE_X);
            AxisRanges[1] = GetRange(HID_USAGES.HID_USAGE_Y);
            AxisRanges[2] = GetRange(HID_USAGES.HID_USAGE_Z);
            AxisRanges[3] = GetRange(HID_USAGES.HID_USAGE_RX);
            AxisRanges[4] = GetRange(HID_USAGES.HID_USAGE_RY);
            AxisRanges[5] = GetRange(HID_USAGES.HID_USAGE_RZ);
            AxisRanges[6] = GetRange(HID_USAGES.HID_USAGE_SL0);
            AxisRanges[7] = GetRange(HID_USAGES.HID_USAGE_SL1);
        }

        public abstract void Execute(Update update);

        private AxisRange GetRange(HID_USAGES axis) {
            var result = new AxisRange();

            Joy.GetVJDAxisMin(DeviceId, axis, ref result.Min);
            Joy.GetVJDAxisMax(DeviceId, axis, ref result.Max);
            result.Dead = result.Max/2;

            return result;
        }

        protected readonly uint DeviceId;
        protected readonly vJoy Joy;

        protected readonly AxisRange[] AxisRanges = new AxisRange[8];

        protected vJoy.JoystickState DeviceState;
    }

    public class vJoyButtonMapping
        : IMapping {

        public vJoyButtonMapping(uint[] buttons, uint deviceId, vJoy joy, ILoggerAdapter logger) {
            _buttons = buttons;
            _deviceId = deviceId;
            _joy = joy;
            _logger = logger;
        }

        public void Execute(Update update) {
            if (_joy.GetVJDStatus(_deviceId) != VjdStat.VJD_STAT_OWN) {
                _logger.Trace($"vJoy device not owned on process {_joy.GetOwnerPid(_deviceId)}.");
                _joy.AcquireVJD(_deviceId);
                _logger.Trace("Device reacquired.");
            }

            foreach (var button in _buttons) {
                _joy.SetBtn(true, _deviceId, button);
                _joy.SetBtn(false, _deviceId, button);
            }
        }

        private vJoy _joy;
        private readonly ILoggerAdapter _logger;
        private readonly uint[] _buttons;
        private uint _deviceId;
        private vJoy.JoystickState _deviceState;

    }
}