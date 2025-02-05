using MvvmHelpers;
using MvvmHelpers.Commands;
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

        private double _left, _top;
        private double _width, _height;

        public double Left { get => _left; set => SetProperty(ref _left, value); }
        public double Top { get => _top; set => SetProperty(ref _top, value); }
        public double Width { get => _width; set => SetProperty(ref _width, value); }
        public double Height { get => _height; set => SetProperty(ref _height, value); }

        #endregion Window Properties

        #region Commands

        public ICommand? KeyDownCommand { get; private set; }
        public ICommand? KeyUpCommand { get; private set; }
        public ICommand? MouseWheelCommand { get; private set; }

        #endregion Commands

        #region Constructor

        public MainWindowViewModel()
        {
            InitializeWindow();
            InitializeCommands();
        }

        private void InitializeWindow()
        {
            Width = Screen.PrimaryScreen?.Bounds.Width ?? 1920;
            Height = Screen.PrimaryScreen?.Bounds.Height ?? 1080;
        }

        private void InitializeCommands()
        {
            KeyDownCommand = new Command<System.Windows.Input.KeyEventArgs>(OnKeyDown);
            KeyUpCommand = new Command<System.Windows.Input.KeyEventArgs>(OnKeyUp);
            MouseWheelCommand = new Command<MouseWheelEventArgs>(OnMouseWheelScrolled);
        }

        #endregion Constructor

        #region Event Handlers

        private void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
        }

        private void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
        }

        private void OnMouseWheelScrolled(MouseWheelEventArgs args)
        {
        }

        #endregion Event Handlers
    }
}