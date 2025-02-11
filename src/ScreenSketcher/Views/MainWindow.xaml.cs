using ScreenSketcher.ViewModels;
using System.Windows;
using System.Windows.Data;

namespace ScreenSketcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = MainWindowViewModel.This;
            InitializeComponent();

            BindingOperations.SetBinding(brdToolbox, VisibilityProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.ToolboxVisibility)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, VisibilityProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Visibility)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, WindowStateProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.WindowState)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, LeftProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Left)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, TopProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Top)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, WidthProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Width)) { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, HeightProperty, new System.Windows.Data.Binding(nameof(MainWindowViewModel.Height)) { Mode = BindingMode.TwoWay });
        }
    }
}