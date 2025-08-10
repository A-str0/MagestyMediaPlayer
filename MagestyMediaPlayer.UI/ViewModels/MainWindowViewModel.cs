namespace MagestyMediaPlayer.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public PlaybackControlViewModel PlaybackControlViewModel { get; }

        public MainWindowViewModel(PlaybackControlViewModel playbackControlViewModel)
        {
            PlaybackControlViewModel = playbackControlViewModel;
        }
    }
}
