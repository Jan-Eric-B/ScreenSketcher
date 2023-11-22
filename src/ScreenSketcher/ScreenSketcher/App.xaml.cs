using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace ScreenSketcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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
                        new ToolStripMenuItem("Settings", null, (s, ev) => { SettingsWindow_Open(); }),
                        new ToolStripMenuItem("Exit", null, (s, ev) => { Current.Shutdown(); })
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

        private static void SettingsWindow_Open()
        {
            SettingsWindow settingsWindow = new();
            settingsWindow.Show();
        }
    }
}
