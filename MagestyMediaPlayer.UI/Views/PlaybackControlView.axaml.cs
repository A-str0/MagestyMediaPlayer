using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MagestyMediaPlayer.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MagestyMediaPlayer.UI.Views;

public partial class PlaybackControlView : UserControl
{
    public PlaybackControlView()
    {
        DataContext = Program.Services.GetRequiredService<PlaybackControlViewModel>();

        InitializeComponent();
    }
}