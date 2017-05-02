
namespace Dillon.Server {
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using NLog;
    using Autofac;
    using Configuration;

    internal static class Program {
        private static Logger _log;

        [STAThread]
        private static int Main(string[] args) {
            try {

                //if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Any())
                //    return 0;

                _log = LogManager.GetCurrentClassLogger();
                _log.Info($"======== Dillon.Server startup v{Assembly.GetExecutingAssembly().GetName().Version}");
                foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
                    _log.Debug(e.Key + ":" + e.Value);

                var container = DependancyRegistrar.RegisterDependancies(args);

                RunApplication(container);
                _log.Info("======== Dillon.Server shutdown");

                return 0;
            } catch (Exception e) {
                _log.Error(e);
                MessageBox.Show($"An exception occured whilst running the application. Check the log file for further information.\n\n{e.Message}",
                    "An exception occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 2;
            }
        }

        private static void RunApplication(IContainer container) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppContext(container));
        }
    }
}