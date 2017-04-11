namespace Dillon.Plugin.vJoy {
    using System.Threading;
    using Common;
    using PluginAPI.V1;
    using vJoyInterfaceWrap;

    public abstract class vJoyMapping {
        protected vJoyMapping(vJoy joy, uint deviceId, ILoggerAdapter logger) {
            Joy = joy;
            DeviceId = deviceId;
            Logger = logger;
        }

        protected readonly vJoy Joy;
        protected readonly uint DeviceId;
        protected readonly ILoggerAdapter Logger;
    }

    public abstract class vJoyAxisMappingBase
        : vJoyMapping, IMapping {

        protected struct AxisRange {
            public long Min;
            public long Max;
            public long Dead;
        }

        protected vJoyAxisMappingBase(vJoy joy, uint deviceId, ILoggerAdapter logger)
            : base(joy, deviceId, logger) {

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

        protected readonly AxisRange[] AxisRanges = new AxisRange[8];
    }

    public class vJoyXAxisMapping
        : vJoyAxisMappingBase {

        public vJoyXAxisMapping(vJoy joy, uint deviceId, ILoggerAdapter logger)
            : base(joy, deviceId, logger) {
        }

        public override void Execute(Update update) {
            if (Joy.GetVJDStatus(DeviceId) != VjdStat.VJD_STAT_OWN)
                Joy.AcquireVJD(DeviceId);

            Joy.SetAxis((int) (AxisRanges[0].Max*update.NormalisedX), DeviceId, HID_USAGES.HID_USAGE_X);
        }
    }

    public class vJoyYAxisMapping
        : vJoyAxisMappingBase {

        public vJoyYAxisMapping(vJoy joy, uint deviceId, ILoggerAdapter logger)
            : base(joy, deviceId, logger) {
        }

        public override void Execute(Update update) {
            if (Joy.GetVJDStatus(DeviceId) != VjdStat.VJD_STAT_OWN) {
                Joy.AcquireVJD(DeviceId);
            }

            Joy.SetAxis((int) (AxisRanges[1].Max*update.NormalisedY), DeviceId, HID_USAGES.HID_USAGE_Y);
        }
    }

    public class vJoyDualAxisMapping
        : vJoyAxisMappingBase {

        public vJoyDualAxisMapping(vJoy joy, uint deviceId, ILoggerAdapter logger)
            : base(joy, deviceId, logger) {
        }

        public override void Execute(Update update) {
            if (Joy.GetVJDStatus(DeviceId) != VjdStat.VJD_STAT_OWN)
                Joy.AcquireVJD(DeviceId);

            Joy.SetAxis((int) (AxisRanges[0].Max*update.NormalisedX), DeviceId, HID_USAGES.HID_USAGE_X);
            Joy.SetAxis((int) (AxisRanges[1].Max*update.NormalisedY), DeviceId, HID_USAGES.HID_USAGE_Y);
        }
    }

    public class vJoyButtonMapping
        : vJoyMapping, IMapping {

        public vJoyButtonMapping(uint[] buttons, vJoy joy, uint deviceId, ILoggerAdapter logger, int delay)
            : base(joy, deviceId, logger) {
            _buttons = buttons;
            _delay = delay;
        }

        public void Execute(Update update) {
            if (Joy.GetVJDStatus(DeviceId) != VjdStat.VJD_STAT_OWN) {
                Logger.Trace($"vJoy device not owned on process {Joy.GetOwnerPid(DeviceId)}.");
                Joy.AcquireVJD(DeviceId);
                Logger.Trace("Device reacquired.");
            }

            foreach (var button in _buttons) {
                Joy.SetBtn(true, DeviceId, button);
                Thread.Sleep(_delay);
                Joy.SetBtn(false, DeviceId, button);
            }
        }

        private readonly int _delay;
        private readonly uint[] _buttons;
    }
}