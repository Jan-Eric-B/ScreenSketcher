using System.Windows;

namespace ScreenSketcher
{
    public partial class App : System.Windows.Application
    {
        private NotifyIcon notifyIcon = new();
        private MainWindow mainWindow = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mainWindow = new();

            notifyIcon = new NotifyIcon()
            {
                Icon = new("Icon.ico"),
                Visible = true,
                Text = "ScreenSketcher",
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items =
                    {
                        new ToolStripMenuItem("Open", null, (s, ev) => { MainWindow_Open(); }),
                        new ToolStripMenuItem("Close", null, (s, ev) => { Current.Shutdown(); })
                    }
                }
            };

            notifyIcon.MouseDown +=
               (sender, args) =>
               {
                   if (args.Button == MouseButtons.Left)
                   {
                       MainWindow_Open();
                   }
               };

            MainWindow_Hide();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon?.Dispose();
            base.OnExit(e);
        }

        private void MainWindow_Open()
        {
            mainWindow.Visibility = Visibility.Visible;
        }

        private void MainWindow_Hide()
        {
            mainWindow.Visibility = Visibility.Hidden;
        }
    }
}