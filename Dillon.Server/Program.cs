
namespace Dillon.Server {
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using NLog;

    internal static class Program {
        private static Logger _log;

        [STAThread]
        private static int Main(string[] args) {
            _log = LogManager.GetCurrentClassLogger();
            _log.Info($"======== Dillon.Server startup v{Assembly.GetExecutingAssembly().GetName().Version}");
            foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
                _log.Debug(e.Key + ":" + e.Value);

            var config = new Configuration();
            ApplyCommandArguments(args, config);

            //plugins

            if (!TryParseConfiguration(config))
                return 1;

            return RunApplication(config);
        }

        private static void ApplyCommandArguments(string[] args, Configuration config) {
            foreach (var arg in args) {
                if (arg == "-d")
                    config.Debug = true;
            }

#if DEBUG
            config.Debug = true;
#endif
            config.UIFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "UI");

            if (config.Debug)
                ApplyDebugMode();
        }

        private static void ApplyDebugMode() {
            //set log level to TRACE
            //set archiveOnStartup to true
        }

        private static int RunApplication(Configuration config) {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new AppContext(config));
                _log.Info("======== Dillon.Server shutdown");
            }
            catch (Exception e) {
                _log.Error(e);
                MessageBox.Show(
                    $"An exception occured whilst running the application. Check the log file for further information.\n{e.Message}",
                    "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 2;
            }
            return 0;
        }

        private static bool TryParseConfiguration(Configuration config) {
            try {
                Configuration.Parse(config);
                return true;
            }
            catch (TypeInitializationException e) {
                _log.Error(e);
                var message = e.InnerException?.Message ?? e.Message;
                MessageBox.Show(
                    $"An exception occured trying to parse one or more of the configuration files.\n{message}",
                    "Exception parsing configuration file(s).", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}