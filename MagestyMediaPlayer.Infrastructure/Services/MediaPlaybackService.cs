using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using MagestyMediaPlayer.Core.Interfaces;
using MagestyMediaPlayer.Core.Models;

namespace MagestyMediaPlayer.Infrastructure.Services
{
    public class MediaPlaybackService : IMediaPlaybackService, IDisposable
    {
        private readonly LibVLC _vlc = new LibVLC();
        private readonly MediaPlayer _mediaPlayer;

        private LoopMode _loopMode;
        private int _currentIndex;

        private readonly List<MediaItem> _queue = new List<MediaItem>();
        public IReadOnlyList<MediaItem> Queue => _queue.AsReadOnly();

        public MediaPlayer MediaPlayer => _mediaPlayer;

        public MediaItem? CurrentMediaItem { get; private set; }

        public MediaPlaybackService()
        {
            _mediaPlayer = new MediaPlayer(_vlc);

            //_mediaPlayer.EndReached += 
        }

        #region Queue
        public async Task NextAsync()
        {
            if (_queue.Count == 0)
            {
                _mediaPlayer.Stop();
                CurrentMediaItem = null;
                return;
            }

            int nextIndex;
            switch (_loopMode)
            {
                case LoopMode.Disable:
                    nextIndex = _currentIndex + 1 < _queue.Count ? _currentIndex + 1 : -1;
                    break;
                case LoopMode.LoopQueue:
                    nextIndex = _currentIndex + 1 < _queue.Count ? _currentIndex + 1 : 0;
                    break;
                case LoopMode.LoopTrack:
                    nextIndex = _currentIndex;
                    break;
                default:
                    nextIndex = -1;
                    break;
            }

            if (nextIndex >= 0 && nextIndex < _queue.Count)
            {
                _currentIndex = nextIndex;
                CurrentMediaItem = _queue[_currentIndex];
                await PlayAsync(CurrentMediaItem);
            }
            else
            {
                CurrentMediaItem = null;
                _currentIndex = -1;
                _mediaPlayer.Stop();
            }
        }

        public async Task PreviousAsync()
        {
            if (_queue.Count == 0 || _currentIndex <= 0)
            {
                // TODO: implement switch case
                if (_loopMode == LoopMode.LoopQueue && _queue.Count > 0)
                {
                    _currentIndex = _queue.Count - 1;
                    CurrentMediaItem = _queue[_currentIndex];
                    await PlayAsync(CurrentMediaItem);
                }
                else
                {
                    _mediaPlayer.Stop();
                }
                return;
            }

            _currentIndex--;
            CurrentMediaItem = _queue[_currentIndex];
            await PlayAsync(CurrentMediaItem);
        }

        public async Task AddToQueueAsync(MediaItem mediaItem)
        {
            ArgumentNullException.ThrowIfNull(mediaItem);
            await Task.Run(() => _queue.Add(mediaItem));
        }

        public async Task AddToQueueAsync(IEnumerable<MediaItem> mediaItems)
        {
            ArgumentNullException.ThrowIfNull(mediaItems);

            var items = mediaItems.ToList();
            if (items.Count == 0) return;

            await Task.Run(() => _queue.AddRange(items));
        }

        public void RemoveFromQueue(MediaItem mediaItem)
        {
            ArgumentNullException.ThrowIfNull(mediaItem);
            int index = _queue.IndexOf(mediaItem);
            if (index >= 0)
            {
                _queue.RemoveAt(index);
                if (index <= _currentIndex && _currentIndex > 0)
                    _currentIndex--;

                if (mediaItem == CurrentMediaItem)
                {
                    CurrentMediaItem = _currentIndex < _queue.Count ? _queue[_currentIndex] : null;
                    if (CurrentMediaItem == null)
                        _mediaPlayer.Stop();
                }
            }
        }

        public void ClearQueue()
        {
            _queue.Clear();
            _currentIndex = -1;
            CurrentMediaItem = null;
            _mediaPlayer.Stop();
        }
        #endregion

        #region Playback
        public async Task PlayAsync(MediaItem mediaItem)
        {
            ArgumentNullException.ThrowIfNull(mediaItem);

            if (!_queue.Contains(mediaItem))
            {
                throw new NotImplementedException("Queue generating when track selected");
            }

            _currentIndex = _queue.IndexOf(mediaItem);
            CurrentMediaItem = mediaItem;

            using var media = new Media(_vlc, mediaItem.SourceUri, FromType.FromPath);
            await Task.Run(() => MediaPlayer.Play(media));
        }

        public void PlayPause()
        {
            MediaPlayer.SetPause(MediaPlayer.IsPlaying);
        }
        #endregion

        #region Setters
        public void SetPosition(float positon)
        {
            positon = Math.Clamp(positon, 0f, 1f);

            MediaPlayer.Position = positon;
        }

        public void SetVolume(int volume)
        {
            MediaPlayer.Volume = volume;
        }

        public void SetLoopMode(LoopMode type)
        {
            _loopMode = type;
        }
        #endregion

        public void Dispose()
        {
            MediaPlayer?.Dispose();
            _vlc?.Dispose();
        }
    }
}
