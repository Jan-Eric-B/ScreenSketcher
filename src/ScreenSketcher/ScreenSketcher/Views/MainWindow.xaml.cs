using ScreenSketcher.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;

namespace ScreenSketcher
{
    public partial class MainWindow : Window
    {
        #region DLL Import

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(
            IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest,
            IntPtr hdcSource, int xSrc, int ySrc, int dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        #endregion DLL Import

        public MainWindow()
        {
            DataContext = MainWindowViewModel.This;
            InitializeComponent();

            BindingOperations.SetBinding(this, VisibilityProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Visibility)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, WindowStateProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.WindowState)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, LeftProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Left)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, TopProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Top)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, WidthProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Width)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, HeightProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Height)) { Mode = BindingMode.TwoWay });
        }
    }
}