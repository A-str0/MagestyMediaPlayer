using Avalonia.Controls;
using MagestyMediaPlayer.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MagestyMediaPlayer.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var serviceProvider = Program.Services;
            DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>();

            var libraryView = this.FindControl<LibraryView>("LibraryView");
            if (libraryView != null)
            {
                libraryView.DataContext = serviceProvider.GetRequiredService<LibraryViewModel>();
            }
        }
    }
}