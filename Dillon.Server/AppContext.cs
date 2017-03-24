namespace Dillon.Server {
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Web.Http;
    using System.Windows.Forms;
    using WindowsInput;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.Hosting;
    using Microsoft.Owin.StaticFiles;
    using NLog;
    using Owin;


    public class AppContext
        : ApplicationContext {
        private readonly Configuration _config;

        public AppContext(Configuration config) {
            _config = config;
            Application.ApplicationExit += OnApplicationExit;
            Initialise();
        }

        private void Initialise() {

            _trayIcon = new NotifyIcon {
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipText = "I noticed that you double-clicked me! What can I do for you?",
                BalloonTipTitle = "You called Master?",
                Text = "Dillon.Server",
                Icon = Resources.TrayIcon
            };

            _trayIcon.DoubleClick += TrayIcon_DoubleClick;

            _trayIconContextMenu = new ContextMenuStrip();
            _closeMenuItem = new ToolStripMenuItem();
            _trayIconContextMenu.SuspendLayout();

            _trayIconContextMenu.Items.AddRange(new ToolStripItem[] {_closeMenuItem});
            _trayIconContextMenu.Name = "_trayIconContextMenu";
            _trayIconContextMenu.Size = new Size(153, 70);

            _closeMenuItem.Name = "_closeMenuItem";
            _closeMenuItem.Size = new Size(152, 22);
            _closeMenuItem.Text = "Exit";
            _closeMenuItem.Click += CloseMenuItem_Click;

            _trayIconContextMenu.ResumeLayout(false);
            _trayIcon.ContextMenuStrip = _trayIconContextMenu;

            var log = LogManager.GetCurrentClassLogger();
            string url = $"{_config.Scheme}://{_config.Domain}:{_config.Port}";
            log.Debug($"Starting webserver listening on {url}");
            WebApp.Start(url, Startup);

            _trayIcon.Visible = true;
        }

        private void Startup(IAppBuilder appBuilder) {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterInstance(_config).As<IConfiguration>();
            builder.RegisterType<InputSimulator>().As<IInputSimulator>();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(config);
            appBuilder.UseWebApi(config);
            var fileSystem = new PhysicalFileSystem(".");

            var options = new FileServerOptions { FileSystem = fileSystem };

            appBuilder.UseFileServer(options);
        }

        private void Terminate() {
            if (_trayIcon != null)
                _trayIcon.Visible = false;

            Application.Exit();
        }

        private void OnApplicationExit(object sender, EventArgs e) {
            Terminate();
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e) {
            //Here you can do stuff if the tray icon is doubleclicked
            _trayIcon.ShowBalloonTip(10000);
        }

        private void CloseMenuItem_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Do you really want to close me?",
                "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
                Application.Exit();
            }
        }

        private NotifyIcon _trayIcon;
        private ContextMenuStrip _trayIconContextMenu;
        private ToolStripMenuItem _closeMenuItem;
    }
}