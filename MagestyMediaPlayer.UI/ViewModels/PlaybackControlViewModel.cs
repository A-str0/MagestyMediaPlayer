using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using MagestyMediaPlayer.Core.Interfaces;
using ReactiveUI;

namespace MagestyMediaPlayer.UI.ViewModels
{
    public class PlaybackControlViewModel : ViewModelBase
    {
        private readonly IMediaPlaybackService _mediaPlaybackService;

        public ReactiveCommand<Unit, Unit> PlayPauseCommand;
        public ReactiveCommand<Unit, Unit> NextCommand;
        public ReactiveCommand<Unit, Unit> PreviousCommand;


        public PlaybackControlViewModel(IMediaPlaybackService mediaPlaybackService)
        {
            _mediaPlaybackService = mediaPlaybackService;

            PlayPauseCommand = ReactiveCommand.Create(PlayPause);
            NextCommand = ReactiveCommand.CreateFromTask(NextAsync);
            PreviousCommand = ReactiveCommand.CreateFromTask(PreviousAsync);
        }

        public void PlayPause() => _mediaPlaybackService?.PlayPause();
        public async Task NextAsync() => await _mediaPlaybackService.NextAsync();
        public async Task PreviousAsync() => await _mediaPlaybackService.PreviousAsync();
    }
}
