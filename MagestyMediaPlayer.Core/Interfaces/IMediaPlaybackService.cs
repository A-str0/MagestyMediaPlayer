using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagestyMediaPlayer.Core.Models;

namespace MagestyMediaPlayer.Core.Interfaces
{
    public enum LoopMode
    {
        Disable,
        LoopQueue,
        LoopTrack,
    }

    public interface IMediaPlaybackService
    {
        public Task NextAsync();
        public Task PreviousAsync();
        public Task AddToQueueAsync(MediaItem mediaItem);
        public Task AddToQueueAsync(IEnumerable<MediaItem> mediaItems);
        public void RemoveFromQueue(MediaItem mediaItem);
        public void ClearQueue();

        public Task PlayAsync(MediaItem media);
        public void PlayPause();

        public void SetVolume(int volume);
        public void SetPosition(float positon);
        public void SetLoopMode(LoopMode type);
    }
}
