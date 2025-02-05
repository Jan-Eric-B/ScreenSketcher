using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows.Media.Imaging;

namespace ScreenSketcher.Services
{
    /// <summary>
    /// Handles saving screenshot images
    /// </summary>
    internal static class FileSystemInfoService
    {
        /// <summary>
        /// Shos save dialog and saves image
        /// </summary>
        public static async Task SaveImageAsync(BitmapSource image)
        {
            VistaSaveFileDialog? saveFileDialog = CreateSaveFileDialog();
            if (saveFileDialog.ShowDialog() != true) return;

            await using FileStream? fileStream = new(saveFileDialog.FileName, FileMode.Create);
            PngBitmapEncoder? encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(fileStream);
        }

        /// <summary>
        /// Creates and configures save file dialog
        /// </summary>
        private static VistaSaveFileDialog CreateSaveFileDialog() => new()
        {
            Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*",
            DefaultExt = "png",
            FileName = $"ScreenSketch - {DateTime.Now:yyyy-MM-dd - HH-mm-ss.FFF}",
            AddExtension = true
        };
    }
}