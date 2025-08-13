using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly LibVLC _vlc;
        private readonly MediaPlayer _mediaPlayer;
        // private readonly IMemoryCache _cache;
        private readonly PlaybackQueue _queue;

        public MediaPlayer MediaPlayer => _mediaPlayer;

        public MediaItem? CurrentQueueItem => _queue.Current;
        public Media? CurrentPlayingMedia => _mediaPlayer.Media;

        public event EventHandler<MediaItem?>? CurrentTrackChanged;

        // public MediaPlaybackService(IMemoryCache cache)
        public MediaPlaybackService()
        {
            LibVLCSharp.Shared.Core.Initialize();

            _vlc = new LibVLC();
            _mediaPlayer = new MediaPlayer(_vlc);
            _mediaPlayer.EndReached += async (s, e) => await OnEndReached();
            // _mediaPlayer.TimeChanged += (s, e) => OnTimeChanged(s, e);

            // _cache = cache;
            _queue = new PlaybackQueue();
            _queue.CurrentItemChanged += async (s, e) => await OnCurrentItemChanged(e);
        }

        public async Task PlayAsync(MediaItem? mediaItem)
        {
            if (mediaItem == null || MediaPlayer == null) return;

            await Task.Run(() =>
            {
                // TODO: move
                Media media = new Media(_vlc, mediaItem.SourceUri);
                _mediaPlayer.Play(media);
            });
        }

        public async Task PlayPauseAsync()
        {
            _mediaPlayer.SetPause(_mediaPlayer.IsPlaying);
        }

        public async Task PlayNextAsync()
        {
            _queue.MoveNext();
        }

        public async Task PlayPreviousAsync()
        {
            _queue.MovePrevious();
        }

        public void SetPosition(float position)
        {
            if (MediaPlayer == null || MediaPlayer.Media != null) return;

            MediaPlayer.Position = position;
        }

        public void SetVolume(int volume)
        {
            if (MediaPlayer == null) return;

            MediaPlayer.Volume = volume;
        }

        public void Mute()
        {
            MediaPlayer.ToggleMute();
            Debug.WriteLine($"{this}: Mute status: {MediaPlayer.Mute}", "debug");
        }

        private async Task OnCurrentItemChanged(MediaItem? item)
        {
            // if (item == null && _queue.Current != null)
            // {
            //     if (item != null)
            //         _cache.Set(item.Id, item, new MemoryCacheEntryOptions
            //         {
            //             SlidingExpiration = TimeSpan.FromMinutes(10),
            //             Size = 1024 // ~1 КБ на MediaItem
            //         });
            // }
            if (item != null)
            {
                using var media = new Media(_vlc, item.SourceUri);
                await Task.Run(() => _mediaPlayer.Play(media));
            }
            else
            {
                _mediaPlayer.Stop();
            }

            CurrentTrackChanged?.Invoke(this, item);
        }

        private async Task OnEndReached()
        {
            await PlayNextAsync();
        }

        // TODO: change or delete
        public void InitializeQueue(IEnumerable<MediaItem> items, MediaItem? selectedItem = null, bool shuffle = false)
        {
            _queue.Initialize(items, selectedItem, shuffle);
        }

        public void Dispose()
        {
            _queue?.Dispose();

            MediaPlayer?.Dispose();
            _vlc?.Dispose();
        }
    }
}