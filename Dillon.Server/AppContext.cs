namespace Dillon.Server {
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Web.Http;
    using System.Windows.Forms;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Common;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.Hosting;
    using Microsoft.Owin.StaticFiles;
    using NLog;
    using Owin;


    public class AppContext
        : ApplicationContext {

        public AppContext(IConfiguration config) {
            _config = config;
            Application.ApplicationExit += OnApplicationExit;
            _log = LogManager.GetCurrentClassLogger();
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

            StartServer();

            _trayIcon.Visible = true;
            _trayIcon.ShowBalloonTip(10000, "Dillon server", "The Dillon server is now listening on port " + _config.Port, ToolTipIcon.Info);

        }

        private void StartServer() {
            try {
                string url = $"{_config.Scheme}://{_config.Domain}:{_config.Port}";
                _log.Debug($"Starting webserver listening on {url}");
                WebApp.Start(url, Startup);
            } catch (Exception e) {
                throw new Exception($"Unable to start web server. {e.InnerMostException().Message}");
            }
        }

        private void Startup(IAppBuilder appBuilder) {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterInstance(_config).As<IConfiguration>();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(config);
            appBuilder.UseWebApi(config);
            var fileSystem = new PhysicalFileSystem(_config.UIFolder);

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
            Terminate();
        }

        private NotifyIcon _trayIcon;
        private ContextMenuStrip _trayIconContextMenu;
        private ToolStripMenuItem _closeMenuItem;
        private Logger _log;
        private readonly IConfiguration _config;
    }
}