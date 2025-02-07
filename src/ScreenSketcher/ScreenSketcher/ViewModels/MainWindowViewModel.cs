using MvvmHelpers.Commands;
using ScreenSketcher.Enums;
using ScreenSketcher.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
        public Visibility Visibility { get => _visibility; set => SetProperty(ref _visibility, value); }

        private Visibility _toolboxVisibility;
        public Visibility ToolboxVisibility { get => _toolboxVisibility; set => SetProperty(ref _toolboxVisibility, value); }

        private WindowState _windowState;
        public WindowState WindowState { get => _windowState; set => SetProperty(ref _windowState, value); }

        private double _left, _top;
        private double _width, _height;
        public double Left { get => _left; set => SetProperty(ref _left, value); }
        public double Top { get => _top; set => SetProperty(ref _top, value); }
        public double Width { get => _width; set => SetProperty(ref _width, value); }
        public double Height { get => _height; set => SetProperty(ref _height, value); }

        private SolidColorBrush _borderColor = new(Colors.Red);

        public SolidColorBrush BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                if (SetProperty(ref _borderColor, value))
                {
                    OnPropertyChanged(nameof(BorderColor));
                }
            }
        }

        private int _borderThickness = 2;

        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (SetProperty(ref _borderThickness, value))
                {
                    OnPropertyChanged(nameof(BorderThickness));
                }
            }
        }

        private System.Windows.Media.Brush _windowBackgroundColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#01FFFFFF"));

        public System.Windows.Media.Brush WindowBackgroundColor
        {
            get => _windowBackgroundColor;
            set
            {
                if (SetProperty(ref _windowBackgroundColor, value))
                {
                    OnPropertyChanged(nameof(WindowBackgroundColor));
                }
            }
        }

        #endregion Window

        private int _drawingThickness = 20;

        public int DrawingThickness
        {
            get => _drawingThickness;
            set
            {
                if (SetProperty(ref _drawingThickness, Math.Max(1, Math.Min(500, value))))
                {
                    UpdateDrawingAttributes();
                    if (CurrentTool == DrawingTool.Eraser)
                    {
                        EraserShape = new RectangleStylusShape(DrawingThickness, DrawingThickness);
                    }
                }
            }
        }

        private DrawingAttributes _drawingAttributes;

        public DrawingAttributes DrawingAttributes
        {
            get => _drawingAttributes;
            private set => SetProperty(ref _drawingAttributes, value);
        }

        private readonly Stack<StrokeCollection> _undoStack = new();
        private readonly Stack<StrokeCollection> _redoStack = new();
        public StrokeCollection Strokes { get; } = [];

        private DrawingTool _currentTool = DrawingTool.Brush;

        public DrawingTool CurrentTool
        {
            get => _currentTool;
            set
            {
                if (SetProperty(ref _currentTool, value))
                {
                    UpdateDrawingAttributes();
                    OnPropertyChanged(nameof(CanvasEditingMode));
                }
            }
        }

        private InkCanvasEditingMode _canvasEditingMode = InkCanvasEditingMode.Ink;

        public InkCanvasEditingMode CanvasEditingMode
        {
            get => _canvasEditingMode;
            set => SetProperty(ref _canvasEditingMode, value);
        }

        private RectangleStylusShape _eraserShape = new(20, 20);

        public RectangleStylusShape EraserShape
        {
            get => _eraserShape;
            set
            {
                _eraserShape = value;
                UpdateDrawingAttributes();
                OnPropertyChanged(nameof(EraserShape));
            }
        }

        #endregion Properties

        #region Commands

        public Command EscapeCommand { get; private set; }
        public AsyncCommand SaveCommand { get; private set; }
        public Command ResetCommand { get; private set; }

        public Command<InkCanvasStrokeCollectedEventArgs> StrokeCollectedCommand { get; private set; }
        public Command UndoCommand { get; private set; }
        public Command RedoCommand { get; private set; }

        public Command MouseWheelCommand { get; private set; }

        public Command<DrawingTool> OnToolSelected { get; private set; }

        #endregion Commands

        #region Constructor

        public MainWindowViewModel()
        {
            InitializeWindow();
            InitializeCommands();
            InitializeTools();
        }

        private void InitializeWindow()
        {
            _left = 0;
            _top = 0;

            (int width, int height) = Win32ScreenCaptureService.GetScreenSize();
            Width = width;
            Height = height;

            _visibility = Visibility.Hidden;
            _windowState = WindowState.Normal;
        }

        private void InitializeCommands()
        {
            // [ESC] Close window
            EscapeCommand = new Command(CloseWindow);

            // [CTRL + S] Save Screenshot
            SaveCommand = new AsyncCommand(SaveScreenshotAsync);

            // [CTRL + N] Reset Drawing
            ResetCommand = new Command(ResetDrawing);

            StrokeCollectedCommand = new Command<InkCanvasStrokeCollectedEventArgs>(OnStrokeCollected);
            UndoCommand = new Command(Undo, () => _undoStack.Count != 0);
            RedoCommand = new Command(Redo, () => _redoStack.Count != 0);

            // Control Brush Size
            MouseWheelCommand = new Command<MouseWheelEventArgs>(HandleMouseWheelScrolled);

            OnToolSelected = new Command<DrawingTool>(tool => CurrentTool = tool);
        }

        private void InitializeTools()
        {
            UpdateDrawingAttributes();
            EraserShape = new RectangleStylusShape(DrawingThickness, DrawingThickness);
        }

        #endregion Constructor

        #region Methods

        private void UpdateDrawingAttributes()
        {
            switch (CurrentTool)
            {
                case DrawingTool.Brush:
                    CanvasEditingMode = InkCanvasEditingMode.Ink;
                    DrawingAttributes = new DrawingAttributes
                    {
                        Color = Colors.Red,
                        Width = DrawingThickness,
                        Height = DrawingThickness,
                        StylusTip = StylusTip.Ellipse
                    };
                    break;

                case DrawingTool.Highlighter:
                    CanvasEditingMode = InkCanvasEditingMode.Ink;
                    DrawingAttributes = new DrawingAttributes
                    {
                        Color = Colors.Red,
                        Width = DrawingThickness * 0.5,
                        Height = DrawingThickness * 2,
                        StylusTip = StylusTip.Rectangle
                    };
                    break;

                case DrawingTool.Eraser:
                    CanvasEditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;
            }
            OnPropertyChanged(nameof(DrawingAttributes));
        }

        private void CloseWindow()
        {
            ToggleVisibility();
        }

        private async Task SaveScreenshotAsync()
        {
            SolidColorBrush? originalBorderColor = BorderColor;
            int originalBorderThickness = BorderThickness;
            Visibility originalToolboxVisibility = ToolboxVisibility;

            try
            {
                BorderColor = new SolidColorBrush(Colors.Transparent);
                BorderThickness = 0;
                ToolboxVisibility = Visibility.Hidden;

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Render);
                await Task.Delay(50);

                BitmapSource screenshot = Win32ScreenCaptureService.CaptureScreen();
                await FileSystemInfoService.SaveImageAsync(screenshot);
            }
            finally
            {
                ResetDrawing();
                BorderColor = originalBorderColor;
                BorderThickness = originalBorderThickness;
                ToolboxVisibility = originalToolboxVisibility;
                ToggleVisibility();
            }
        }

        public void ResetDrawing()
        {
            Strokes.Clear();
        }

        private void ToggleVisibility()
        {
            if (Visibility == Visibility.Visible)
            {
                ResetDrawing();
                Visibility = Visibility.Hidden;
            }
            else
            {
                ResetDrawing();
                Visibility = Visibility.Visible;
                WindowState = WindowState.Normal;
            }
        }

        private void HandleMouseWheelScrolled(MouseWheelEventArgs args)
        {
            // Increases and decreases drawing thickness
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                DrawingThickness += args.Delta > 0 ? 1 : -1;
            }
        }

        #region Undo/Redo Functions

        private void Undo()
        {
            if (_undoStack.Count == 0) return;

            // Save current for redo
            _redoStack.Push(new StrokeCollection(Strokes));

            // Restore previous
            Strokes.Clear();
            foreach (Stroke stroke in _undoStack.Pop())
            {
                Strokes.Add(stroke);
            }
        }

        private void Redo()
        {
            if (_redoStack.Count == 0) return;

            // Save current
            _undoStack.Push(new StrokeCollection(Strokes));

            // Restore undone state
            Strokes.Clear();
            foreach (Stroke stroke in _redoStack.Pop())
            {
                Strokes.Add(stroke);
            }
        }

        private void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            _undoStack.Push(new StrokeCollection(Strokes.Take(Strokes.Count - 1)));
            _redoStack.Clear();
        }

        #endregion Undo/Redo Functions

        #endregion Methods
    }
}