namespace Dillon.Server {
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using JsonConfig;
    using Microsoft.Owin.Hosting;

    public class AppContext
        : ApplicationContext {

        public AppContext() {
            Application.ApplicationExit += OnApplicationExit;
            InitializeComponent();
            _trayIcon.Visible = true;
        }

        private void InitializeComponent()
        {
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

            WebApp.Start<Startup>($"{Config.Global.Scheme}://{Config.Global.Domain}:{Config.Global.Port}");
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
            _trayIcon.Visible = false;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            //Here you can do stuff if the tray icon is doubleclicked
            _trayIcon.ShowBalloonTip(10000);
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to close me?",
                "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private NotifyIcon _trayIcon;
        private ContextMenuStrip _trayIconContextMenu;
        private ToolStripMenuItem _closeMenuItem;
    }
}