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
    public class MediaPlaybackService : IMediaPlaybackService, IDisposable
    {
        private readonly LibVLC _vlc = new LibVLC();
        private readonly MediaPlayer _mediaPlayer;

        private PlaybackQueue<MediaItem> _queue;

        public MediaPlayer MediaPlayer => _mediaPlayer;

        public MediaPlaybackService()
        {
            _mediaPlayer = new MediaPlayer(_vlc);
            _queue = new PlaybackQueue<MediaItem>();

            //_mediaPlayer.EndReached += 
        }

        public void NextMediaItem()
        {
            _queue.ToNext();
        }

        public void AddMediaItems(ICollection<MediaItem> items)
        {
            foreach (MediaItem item in items)
            {
                _queue.AddLast(item);
            }
        }

        // TEMP!!!
        public void CreateQueue(ICollection<MediaItem> items)
        {
            AddMediaItems(items);
        }

        public void Dispose()
        {
            _queue?.Dispose();

            MediaPlayer?.Dispose();
            _vlc?.Dispose();
        }
    }
}