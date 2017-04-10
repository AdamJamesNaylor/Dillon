
namespace Dillon.Server {
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Windows.Forms;
    using WindowsInput;
    using Common;
    using NLog;
    using Input;
    using Mappings;

    internal static class Program {
        private static Logger _log;

        [STAThread]
        private static int Main(string[] args) {
            try {

                _log = LogManager.GetCurrentClassLogger();
                _log.Info($"======== Dillon.Server startup v{Assembly.GetExecutingAssembly().GetName().Version}");
                foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
                    _log.Debug(e.Key + ":" + e.Value);

                var simulator = new InputSimulator();
                var keyboardSimulator = new KeyboardSimulatorAdapter(simulator.Keyboard);
                var mouseSimulator = new MouseSimulatorAdapter(simulator.Mouse);
                var coreMappingFactory = new CoreMappingFactory();
                coreMappingFactory.RegisterDependancy(keyboardSimulator);
                coreMappingFactory.RegisterDependancy(mouseSimulator);
                var configurator = new Configurator(coreMappingFactory);
                var config = configurator.Configure(args);
                return RunApplication(config);

            } catch (Exception e) {
                _log.Error(e);
                MessageBox.Show($"An exception occured whilst running the application. Check the log file for further information.\n\n{e.Message}",
                    "An exception occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 2;
            }
        }

        private static int RunApplication(Configuration config) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppContext(config));
            _log.Info("======== Dillon.Server shutdown");

            return 0;
        }
    }
}