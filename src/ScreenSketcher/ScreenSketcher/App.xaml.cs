using ScreenSketcher.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ScreenSketcher
{
    public partial class App : System.Windows.Application, IDisposable
    {
        private bool _disposed;
        private readonly NotifyIcon _notifyIcon;
        private readonly MainWindow _mainWindow;
        private readonly Icon _trayIcon;
        private readonly HwndSource _source;
        private const int HOTKEY_ID = 9000;

        [DllImport("User32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        public App()
        {
            _mainWindow = new();

            _trayIcon = new(fileName: "Icon.ico");
            _notifyIcon = CreateNotifyIcon();
            _notifyIcon.MouseDown += OnNotifyIconMouseDown;

            // Create message-only window for hotkey registration
            WindowInteropHelper? wndHelper = new(_mainWindow);
            HwndSource? source = HwndSource.FromHwnd(wndHelper.EnsureHandle());
            _source = source;
            source.AddHook(HwndHook);

            RegisterHotKey();
            MainWindow_Hide();
        }

        #region Hotkey

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void RegisterHotKey()
        {
            WindowInteropHelper? wndHelper = new(_mainWindow);

            const uint VK_F8 = 0x77;  // F8
            const uint MOD_CTRL = 0x0002; // CTRL

            if (!RegisterHotKey(wndHelper.Handle, HOTKEY_ID, MOD_CTRL, VK_F8))
            {
                // handle error
                System.Windows.MessageBox.Show("Failed to register hotkey (Ctrl+F8). The application may not work as expected.");
            }
        }

        private void UnregisterHotKey()
        {
            if (_mainWindow != null)
            {
                WindowInteropHelper? wndHelper = new(_mainWindow);
                UnregisterHotKey(wndHelper.Handle, HOTKEY_ID);
            }
        }

        private void OnHotKeyPressed()
        {
            if (_mainWindow.Visibility == Visibility.Hidden)
            {
                MainWindow_Open();
            }
        }

        #endregion Hotkey

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
                    new ToolStripMenuItem("Open (CTRL + F8)", null, (s, ev) => MainWindow_Open()),
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
                UnregisterHotKey();
                _source?.Dispose();
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