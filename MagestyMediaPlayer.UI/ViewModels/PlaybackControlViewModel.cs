using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using MagestyMediaPlayer.Core.Interfaces;
using MagestyMediaPlayer.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace MagestyMediaPlayer.UI.ViewModels
{
    public class PlaybackControlViewModel : ViewModelBase
    {
        private readonly MediaPlaybackService _mediaPlaybackService;

        public ReactiveCommand<Unit, Unit> PlayPauseCommand { get; }
        public ReactiveCommand<Unit, Unit> NextCommand { get; }
        public ReactiveCommand<Unit, Unit> PreviousCommand { get; }


        public PlaybackControlViewModel()
        {
            IServiceProvider serviceProvider = Program.Services.CreateScope().ServiceProvider;

            _mediaPlaybackService = serviceProvider.GetRequiredService<MediaPlaybackService>();

            PlayPauseCommand = ReactiveCommand.CreateFromTask(PlayPause);
            NextCommand = ReactiveCommand.CreateFromTask(NextAsync);
            PreviousCommand = ReactiveCommand.CreateFromTask(PreviousAsync);
        }

        public async Task PlayPause() => await _mediaPlaybackService.PlayPauseAsync(); //_mediaPlaybackService?.PlayPause();
        public async Task NextAsync() => await _mediaPlaybackService.PlayNextAsync(); //await _mediaPlaybackService.NextAsync();
        public async Task PreviousAsync() => await _mediaPlaybackService.PlayPreviousAsync(); //await _mediaPlaybackService.PreviousAsync();
    }
}
