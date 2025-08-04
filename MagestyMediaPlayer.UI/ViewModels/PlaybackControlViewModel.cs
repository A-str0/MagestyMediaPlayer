using System;
using System.Collections.Generic;
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

        public ReactiveCommand<Unit, Unit> PlayPauseCommand;
        public ReactiveCommand<Unit, Unit> NextCommand;
        public ReactiveCommand<Unit, Unit> PreviousCommand;


        public PlaybackControlViewModel()
        {
            IServiceProvider serviceProvider = Program.Services.CreateScope().ServiceProvider;

            _mediaPlaybackService = serviceProvider.GetRequiredService<MediaPlaybackService>();

            PlayPauseCommand = ReactiveCommand.Create(PlayPause);
            NextCommand = ReactiveCommand.CreateFromTask(NextAsync);
            PreviousCommand = ReactiveCommand.CreateFromTask(PreviousAsync);
        }

        public void PlayPause() => Console.WriteLine("play :|"); //_mediaPlaybackService?.PlayPause();
        public async Task NextAsync() => Console.WriteLine("next :)"); //await _mediaPlaybackService.NextAsync();
        public async Task PreviousAsync() => Console.WriteLine("prev :()"); //await _mediaPlaybackService.PreviousAsync();
    }
}
