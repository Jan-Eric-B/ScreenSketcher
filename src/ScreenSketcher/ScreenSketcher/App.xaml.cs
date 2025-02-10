using ScreenSketcher.ViewModels;
using System.Windows;

namespace ScreenSketcher
{
    public partial class App : System.Windows.Application, IDisposable
    {
        private bool _disposed;
        private readonly NotifyIcon _notifyIcon;
        private readonly MainWindow _mainWindow;
        private readonly Icon _trayIcon;

        public App()
        {
            _mainWindow = new();

            _trayIcon = new(fileName: "Icon.ico");
            _notifyIcon = CreateNotifyIcon();
            _notifyIcon.MouseDown += OnNotifyIconMouseDown;

            MainWindow_Hide();
        }

        #region NotifyIcon

        private NotifyIcon CreateNotifyIcon() => new()
        {
            Icon = _trayIcon,
            Visible = true,
            Text = "ScreenSketcher",
            ContextMenuStrip = new()
            {
                Items =
                {
                    new ToolStripMenuItem("Open", null, (s, ev) => MainWindow_Open()),
                    new ToolStripMenuItem("Close", null, (s, ev) => Current.Shutdown())
                }
            },
        };

        private void OnNotifyIconMouseDown(object? sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                MainWindow_Open();
            }
        }

        #endregion NotifyIcon

        #region Window State

        private void MainWindow_Open()
        {
            MainWindowViewModel.This.ResetDrawing();
            _mainWindow.Visibility = Visibility.Visible;
        }

        private void MainWindow_Hide()
        {
            _mainWindow.Visibility = Visibility.Hidden;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();
            base.OnExit(e);
        }

        #endregion Window State

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _notifyIcon.Dispose();
                _trayIcon.Dispose();
                _mainWindow.Close();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Dispose
    }
}