using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MagestyMediaPlayer.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MagestyMediaPlayer.UI.Views;

public partial class LibraryView : UserControl
{
    public LibraryView()
    {
        DataContext = Program.Services.GetRequiredService<LibraryViewModel>();

        InitializeComponent();
    }
}