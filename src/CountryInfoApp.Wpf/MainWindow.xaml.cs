using System.Windows;
using CountryInfoApp.Presentation.ViewModels;

namespace CountryInfoApp.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
