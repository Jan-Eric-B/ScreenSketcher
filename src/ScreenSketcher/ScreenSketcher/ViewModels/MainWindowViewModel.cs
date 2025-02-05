using MvvmHelpers.Commands;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace ScreenSketcher.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Singleton

        private static readonly MainWindowViewModel _this = new();
        public static MainWindowViewModel This => _this;

        #endregion Singleton

        #region Properties

        #region Window

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

        #endregion Window

        private int _drawingThickness = 20;

        public int DrawingThickness
        {
            get => _drawingThickness;
            set
            {
                if (SetProperty(ref _drawingThickness, Math.Max(1, Math.Min(500, value))))
                {
                    OnPropertyChanged(nameof(DrawingAttributes));
                }
            }
        }

        private DrawingAttributes _drawingAttributes;

        public DrawingAttributes DrawingAttributes
        {
            get
            {
                _drawingAttributes = new DrawingAttributes
                {
                    Color = Colors.Red,
                    Width = DrawingThickness,
                    Height = DrawingThickness
                };
                return _drawingAttributes;
            }
        }

        public StrokeCollection Strokes { get; } = new StrokeCollection();

        #endregion Properties

        #region Commands

        public Command EscapeCommand { get; private set; }
        public Command SaveCommand { get; private set; }
        public Command ResetCommand { get; private set; }

        public Command MouseWheelCommand { get; private set; }

        #endregion Commands

        #region Constructor

        public MainWindowViewModel()
        {
            InitializeWindow();
            InitializeCommands();
        }

        private void InitializeWindow()
        {
            _left = 0;
            _top = 0;

            Width = Screen.PrimaryScreen?.Bounds.Width ?? 1920;
            Height = Screen.PrimaryScreen?.Bounds.Height ?? 1080;

            _visibility = Visibility.Hidden;
            _windowState = WindowState.Normal;
        }

        private void InitializeCommands()
        {
            // [ESC] Close window
            EscapeCommand = new Command(CloseWindow);

            // [CTRL + S] Save Screenshot
            SaveCommand = new Command(SaveScreenshot);

            // [CTRL + N] Reset Drawing
            ResetCommand = new Command(ResetDrawing);

            // Control Brush Size
            MouseWheelCommand = new Command<MouseWheelEventArgs>(HandleMouseWheelScrolled);
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
            Strokes.Clear();
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

        private void HandleMouseWheelScrolled(MouseWheelEventArgs args)
        {
            // Increases and decreases drawing thickness
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                DrawingThickness += args.Delta > 0 ? 1 : -1;
            }
        }

        #endregion Event Handlers
    }
}