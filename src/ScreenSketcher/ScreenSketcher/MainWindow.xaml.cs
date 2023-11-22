using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenSketcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

        #endregion

        private SolidColorBrush OriginalBorderBrush = new();
        private Thickness OriginalBorderThickness = new();

        private int ScrollValue = 20;
        private int DrawingAttributesDimension;

        public MainWindow()
        {
            InitializeComponent();

            this.Left = 0;
            this.Top = 0;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;

            SetDrawingPointDimensions();

            this.KeyDown += KeyEvents_Shortcuts;
            this.MouseWheel += OnMouseWheelScrolled;
        }

        private void OnMouseWheelScrolled(object sender, MouseWheelEventArgs e)
        {
            // Check if CTRL key is held down
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ScrollValue += e.Delta > 0 ? 5 : -5;
                SetDrawingPointDimensions();

                //TEMP
                MyLabel.Content = $"Scroll Value: {ScrollValue}";
            }
        }

        private void SetDrawingPointDimensions()
        {
            DrawingAttributesDimension = ScrollValue;
            // sets the height and width of the drawing point.
            if (DrawingAttributesDimension < 5) DrawingAttributesDimension = 5;
            if (DrawingAttributesDimension > 500) DrawingAttributesDimension = 500;

            drawingInkCanvas.DefaultDrawingAttributes.Color = Colors.Blue;
            drawingInkCanvas.DefaultDrawingAttributes.Width = DrawingAttributesDimension;
            drawingInkCanvas.DefaultDrawingAttributes.Height = DrawingAttributesDimension;
            drawingInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }



        protected override void OnClosing(CancelEventArgs e)
        {
            // Minimize to tray instead of closing
            e.Cancel = true; // Prevents the window from closing
            this.Visibility = Visibility.Hidden; // Hide the window
        }

        private void KeyEvents_Shortcuts(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // (ESC) Closes window
            if (e.Key == Key.Escape)
            {
                ToggleVisibility();
            }
            // (S + CTRL) Save Screenshot
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SaveScreenshot();
            }
            // (N + CTRL) Resetting the drawn image
            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Drawing_Reset();
            }
        }

        private void ToggleVisibility()
        {
            // If the window is visible, hide it; otherwise, show it
            if (this.Visibility == Visibility.Visible)
            {
                this.Visibility = Visibility.Hidden;
                Drawing_Reset();
            }
            else
            {
                this.Visibility = Visibility.Visible;
                this.WindowState = WindowState.Normal; // To ensure it's not minimized
            }
        }

        private void Drawing_Reset()
        {
            drawingInkCanvas.Strokes.Clear();
        }

        private async Task SaveScreenshot()
        {
            // Save original border properties to restore later
            OriginalBorderBrush = mainBorder.BorderBrush as SolidColorBrush;
            OriginalBorderThickness = mainBorder.BorderThickness;

            // Make the border transparent and remove its thickness
            mainBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
            mainBorder.BorderThickness = new Thickness(0);

            // Allow the UI to update
            await Task.Delay(100);  // This delay might need adjustment

            // Capture the screen now that the border is invisible
            CaptureScreen();  // This is your existing method to capture the screen

            // Restore original border properties after the screenshot is taken
            mainBorder.BorderBrush = OriginalBorderBrush;
            mainBorder.BorderThickness = OriginalBorderThickness;
        }

        private static void CaptureScreen()
        {
            // Get the size of the screen
            int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            int screenHeight = (int)SystemParameters.PrimaryScreenHeight;

            // Get the device context of the desktop
            IntPtr hDesktopWnd = GetDesktopWindow();
            IntPtr hDesktopDC = GetWindowDC(hDesktopWnd);
            IntPtr hMemoryDC = CreateCompatibleDC(hDesktopDC);

            // Create a compatible bitmap and select it into the memory DC
            IntPtr hBitmap = CreateCompatibleBitmap(hDesktopDC, screenWidth, screenHeight);
            IntPtr hOldBitmap = SelectObject(hMemoryDC, hBitmap);

            // BitBlt the desktop DC to the memory DC (i.e., copy the screen to the bitmap)
            bool success = BitBlt(hMemoryDC, 0, 0, screenWidth, screenHeight, hDesktopDC, 0, 0, 0x00CC0020 /* SRCCOPY */);

            // Select the old bitmap back into the memory DC and clean up
            SelectObject(hMemoryDC, hOldBitmap);
            ReleaseDC(hDesktopWnd, hDesktopDC);
            DeleteObject(hMemoryDC);

            if (!success)
            {
                // If BitBlt failed, clean up and throw an exception
                DeleteObject(hBitmap);
                throw new System.ComponentModel.Win32Exception();
            }

            // At this point, hBitmap is a handle to a bitmap containing a screenshot of the entire screen

            // Convert the HBitmap to a WPF-friendly BitmapSource
            BitmapSource? imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            // Now, you can save imageSource to a file using a BitmapEncoder

            // Encode the BitmapSource as a PNG and save it to a file
            PngBitmapEncoder? encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(imageSource));

            SaveFile(encoder);

            DeleteObject(hBitmap);
        }


        private static void SaveFile(PngBitmapEncoder encoder)
        {
            VistaSaveFileDialog saveFileDialog = new()
            {
                Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*",
                DefaultExt = "png",
                FileName = $"ScreenSketch - {DateTime.Now:yyyy-MM-dd - HH-mm-ss.FFF}",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                using FileStream? fileStream = new(filename, FileMode.Create);
                encoder.Save(fileStream);
            }
        }
    }
}
