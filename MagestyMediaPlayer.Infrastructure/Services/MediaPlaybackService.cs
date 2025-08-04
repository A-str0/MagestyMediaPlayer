using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using MagestyMediaPlayer.Core.Interfaces;
using MagestyMediaPlayer.Core.Models;
using MagestyMediaPlayer.Core.Services;

namespace MagestyMediaPlayer.Infrastructure.Services
{
    public class MediaPlaybackService : IDisposable
    {
        private readonly static PlaybackQueue<MediaItem> _queue = new PlaybackQueue<MediaItem>();

        private readonly LibVLC _vlc;
        private readonly MediaPlayer _mediaPlayer;

        public MediaPlayer MediaPlayer => _mediaPlayer;

        public MediaItem? CurrentQueueItem => _queue.CurrentItem;
        public Media? CurrentPlayingMedia => _mediaPlayer.Media;


        public MediaPlaybackService()
        {
            LibVLCSharp.Shared.Core.Initialize();

            _vlc = new LibVLC();
            _mediaPlayer = new MediaPlayer(_vlc);

            // _queue.Changed += OnQueueChanged;
        }

        public async Task PlayAsync(MediaItem mediaItem)
        {
            _queue.AddNext(mediaItem);

            await Task.Run(() =>
            {
                Media media = new Media(_vlc, mediaItem.SourceUri);
                _mediaPlayer.Play(media);
            });
        }

        public void Dispose()
        {
            _queue?.Dispose();

            MediaPlayer?.Dispose();
            _vlc?.Dispose();
        }
    }
}