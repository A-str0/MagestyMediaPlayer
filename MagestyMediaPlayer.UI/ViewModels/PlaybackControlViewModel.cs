using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using MagestyMediaPlayer.Core.Interfaces;
using MagestyMediaPlayer.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace MagestyMediaPlayer.UI.ViewModels
{
    public class PlaybackControlViewModel : ViewModelBase
    {
        private readonly MediaPlaybackService _mediaPlaybackService;

        public string TrackTitle {
            get
            {
                return _mediaPlaybackService.CurrentQueueItem?.Title??"TITLE PLACEHOLDER";
            }
        }
        public string TrackArtist
        {
            get
            {
                return _mediaPlaybackService.CurrentQueueItem?.Artist??"ARTIST PLACEHOLDER";
            }
        }

        private double _trackPosition;
        public double TrackPosition
        {
            get => _trackPosition;
            set
            {
                if (Math.Abs(_trackPosition - value) > 0.001)
                {
                    _trackPosition = value;
                    this.RaiseAndSetIfChanged(ref _trackPosition, value);

                    if (_mediaPlaybackService != null && !_updatingFromPlayer)
                    {
                        _mediaPlaybackService.SetPosition((float)value);
                    }
                }
            }
        }

        public ReactiveCommand<Unit, Unit> PlayPauseCommand { get; }
        public ReactiveCommand<Unit, Unit> NextCommand { get; }
        public ReactiveCommand<Unit, Unit> PreviousCommand { get; }

        private readonly DispatcherTimer _progressTimer;
        private bool _updatingFromPlayer;

        public PlaybackControlViewModel()
        {
            IServiceProvider serviceProvider = Program.Services.CreateScope().ServiceProvider;

            _mediaPlaybackService = serviceProvider.GetRequiredService<MediaPlaybackService>();
            _mediaPlaybackService.CurrentTrackChanged += (s, e) =>
            {
                this.RaisePropertyChanged(nameof(TrackTitle));
                this.RaisePropertyChanged(nameof(TrackArtist));
            };
            _progressTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            _progressTimer.Tick += (s, e) =>
            {
                if (_mediaPlaybackService.MediaPlayer?.IsPlaying == true)
                {
                    _updatingFromPlayer = true;
                    TrackPosition = _mediaPlaybackService.MediaPlayer.Position;
                    _updatingFromPlayer = false;
                }
            };
            _progressTimer.Start();

            PlayPauseCommand = ReactiveCommand.CreateFromTask(PlayPause);
            NextCommand = ReactiveCommand.CreateFromTask(NextAsync);
            PreviousCommand = ReactiveCommand.CreateFromTask(PreviousAsync);
        }

        public async Task PlayPause() => await _mediaPlaybackService.PlayPauseAsync(); //_mediaPlaybackService?.PlayPause();
        public async Task NextAsync() => await _mediaPlaybackService.PlayNextAsync(); //await _mediaPlaybackService.NextAsync();
        public async Task PreviousAsync() => await _mediaPlaybackService.PlayPreviousAsync(); //await _mediaPlaybackService.PreviousAsync();
    }
}
