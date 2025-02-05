using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows;
using System.Windows.Input;

namespace ScreenSketcher.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {
        #region Singleton

        private static readonly MainWindowViewModel _this = new();
        public static MainWindowViewModel This => _this;

        #endregion Singleton

        #region Window Properties

        private Visibility _visibility;
        private WindowState _windowState;
        private double _left, _top;
        private double _width, _height;

        public Visibility Visibility { get => _visibility; set => SetProperty(ref _visibility, value); }
        public WindowState WindowState { get => _windowState; set => SetProperty(ref _windowState, value); }
        public double Left { get => _left; set => SetProperty(ref _left, value); }
        public double Top { get => _top; set => SetProperty(ref _top, value); }
        public double Width { get => _width; set => SetProperty(ref _width, value); }
        public double Height { get => _height; set => SetProperty(ref _height, value); }

        #endregion Window Properties

        #region Commands

        public Command EscapeCommand { get; private set; }
        public Command SaveCommand { get; private set; }
        public Command ResetCommand { get; private set; }

        #endregion Commands

        #region Constructor

        public MainWindowViewModel()
        {
            InitializeWindow();
            InitializeCommands();

            _visibility = Visibility.Hidden;
            _windowState = WindowState.Normal;
            _left = 0;
            _top = 0;
        }

        private void InitializeWindow()
        {
            Width = Screen.PrimaryScreen?.Bounds.Width ?? 1920;
            Height = Screen.PrimaryScreen?.Bounds.Height ?? 1080;
        }

        private void InitializeCommands()
        {
            // [ESC] Close window
            EscapeCommand = new Command(CloseWindow);

            // [CTRL + S] Save Screenshot
            SaveCommand = new Command(SaveScreenshot);

            // [CTRL + N] Reset drawing
            ResetCommand = new Command(ResetDrawing);
        }

        #endregion Constructor

        #region Methods

        private void CloseWindow()
        {
            ToggleVisibility();
        }

        private void SaveScreenshot()
        {
            // TODO Save Screenshot
        }

        private void ResetDrawing()
        {
            // TODO Reset Drawing
        }

        private void ToggleVisibility()
        {
            if (Visibility == Visibility.Visible)
            {
                Visibility = Visibility.Hidden;
                // TODO Reset Drawing
            }
            else
            {
                Visibility = Visibility.Visible;
                WindowState = WindowState.Normal;
            }
        }

        #endregion Methods

        #region Event Handlers

        private void HandleKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            // TODO Key Shift -> Enable Straigt line
        }

        private void HandleKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            // TODO Key Shift -> Disable Straigt line
        }

        private void HandleMouseWheelScrolled(MouseWheelEventArgs args)
        {
            // TODO Set Drawing Point Dimensions (if CTRL is held down)
        }

        #endregion Event Handlers
    }
}