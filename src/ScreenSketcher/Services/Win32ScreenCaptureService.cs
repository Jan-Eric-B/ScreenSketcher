using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ScreenSketcher.Services
{
    /// <summary>
    /// Win32-based screen capture service
    /// </summary>
    public static class Win32ScreenCaptureService
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

        /// <summary>
        /// Captures screen and returns it as BitmapSource
        /// </summary>
        public static BitmapSource CaptureScreen()
        {
            (int width, int height) screenSize = GetScreenSize();
            nint desktopWindow = GetDesktopWindow();
            nint desktopDC = GetWindowDC(desktopWindow);
            nint memoryDC = CreateCompatibleDC(desktopDC);

            try
            {
                return CaptureScreenToImage(desktopDC, memoryDC, screenSize);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An Error occured: " + ex);
                throw;
            }
            finally
            {
                ReleaseDC(desktopWindow, desktopDC);
                DeleteObject(memoryDC);
            }
        }

        /// <summary>
        /// Returns screen dimensions
        /// </summary>
        public static (int width, int height) GetScreenSize() => ((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);

        /// <summary>
        /// Captures screen using Win32 APIs
        /// </summary>
        private static BitmapSource CaptureScreenToImage(IntPtr desktopDC, IntPtr memoryDC, (int width, int height) screenSize)
        {
            nint bitmap = CreateCompatibleBitmap(desktopDC, screenSize.width, screenSize.height);
            nint oldBitmap = SelectObject(memoryDC, bitmap);

            try
            {
                if (!BitBlt(memoryDC, 0, 0, screenSize.width, screenSize.height, desktopDC, 0, 0, 0x00CC0020))
                {
                    throw new Win32Exception();
                }

                return Imaging.CreateBitmapSourceFromHBitmap(bitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An Error occured: " + ex);
                throw;
            }
            finally
            {
                SelectObject(memoryDC, oldBitmap);
                DeleteObject(bitmap);
            }
        }
    }
}