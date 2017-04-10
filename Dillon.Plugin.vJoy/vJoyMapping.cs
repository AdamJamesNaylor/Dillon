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
            DeviceState = new vJoy.JoystickState {
                bDevice = (byte) DeviceId,
                AxisX = (int) (AxisRanges[0].Max*update.NormalisedX),
                AxisY = (int) AxisRanges[1].Dead,
                AxisZ = (int) AxisRanges[2].Dead,
                AxisXRot = (int) AxisRanges[3].Dead,
                AxisYRot = (int) AxisRanges[4].Dead,
                AxisZRot = (int) AxisRanges[5].Dead
            };

            if (!Joy.UpdateVJD(DeviceId, ref DeviceState))
                Joy.AcquireVJD(DeviceId);
        }
    }

    public class vJoyYAxisMapping
        : vJoyAxisMappingBase {

        public vJoyYAxisMapping(uint vDeviceId, vJoy joy)
            : base(vDeviceId, joy) { }

        public override void Execute(Update update) {
            DeviceState = new vJoy.JoystickState {
                bDevice = (byte) DeviceId,
                AxisX = (int) AxisRanges[0].Dead,
                AxisY = (int) (AxisRanges[1].Max*update.NormalisedY),
                AxisZ = (int) AxisRanges[2].Dead,
                AxisXRot = (int) AxisRanges[3].Dead,
                AxisYRot = (int) AxisRanges[4].Dead,
                AxisZRot = (int) AxisRanges[5].Dead
            };

            if (!Joy.UpdateVJD(DeviceId, ref DeviceState))
                Joy.AcquireVJD(DeviceId);
        }
    }

    public class vJoyDualAxisMapping
        : vJoyAxisMappingBase {

        public vJoyDualAxisMapping(uint vDeviceId, vJoy joy)
            : base(vDeviceId, joy) { }

        public override void Execute(Update update) {
            DeviceState = new vJoy.JoystickState {
                bDevice = (byte) DeviceId,
                AxisX = (int) (AxisRanges[0].Max*update.NormalisedX),
                AxisY = (int) (AxisRanges[1].Max*update.NormalisedY),
                AxisZ = (int) AxisRanges[2].Dead,
                AxisXRot = (int) AxisRanges[3].Dead,
                AxisYRot = (int) AxisRanges[4].Dead,
                AxisZRot = (int) AxisRanges[5].Dead
            };

            if (!Joy.UpdateVJD(DeviceId, ref DeviceState))
                Joy.AcquireVJD(DeviceId);
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

            //Execute(new Update {NormalisedY = (float) AxisRanges[1].Dead/AxisRanges[1].Max});
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

        public vJoyButtonMapping(int[] buttons, uint deviceId, vJoy joy) {
            foreach (var button in buttons) {
                _buttons |= (uint)(0x1 << (button - 1));
            }
            _deviceId = deviceId;
            _joy = joy;
        }

        public void Execute(Update update) {
            _deviceState = new vJoy.JoystickState {
                Buttons = _buttons
            };

            if (!_joy.UpdateVJD(_deviceId, ref _deviceState))
                _joy.AcquireVJD(_deviceId);

            _deviceState = new vJoy.JoystickState {
                Buttons = 0
            };
            _joy.UpdateVJD(_deviceId, ref _deviceState);
        }

        private vJoy _joy;
        private readonly uint _buttons;
        private uint _deviceId;
        private vJoy.JoystickState _deviceState;

    }
}