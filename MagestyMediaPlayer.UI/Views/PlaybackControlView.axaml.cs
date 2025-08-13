using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using MagestyMediaPlayer.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MagestyMediaPlayer.UI.Views;

public partial class PlaybackControlView : UserControl
{
    private bool _overAnchor;
    private bool _overPopup;

    public PlaybackControlView()
    {
        DataContext = Program.Services.GetRequiredService<PlaybackControlViewModel>();

        InitializeComponent();

        VolumeButton.PointerEntered += (_, __) =>
        {
            _overAnchor = true;
            TryShowVolume();
        };
        VolumeButton.PointerExited += (_, __) =>
        {
            _overAnchor = false;
            TryHideVolume();
        };

        if (VolumePopup.Child == null) return;

        VolumePopup.Child.PointerEntered += (_, __) =>
        {
            _overPopup = true;
            TryShowVolume();
        };
        VolumePopup.Child.PointerExited += (_, __) =>
        {
            _overPopup = false;
            TryHideVolume();
        };
    }

    private void TryShowVolume()
    {
        DispatcherTimer.RunOnce(() =>
        {
            if (_overAnchor || _overPopup)
                VolumePopup.IsOpen = true;
        }, System.TimeSpan.FromMilliseconds(250));
    }

    private void TryHideVolume()
    {
        DispatcherTimer.RunOnce(() =>
        {
            if (!_overAnchor && !_overPopup)
                VolumePopup.IsOpen = false;
        }, System.TimeSpan.FromMilliseconds(250));
    }
}