using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagestyMediaPlayer.Core.Models;

namespace MagestyMediaPlayer.Core.Interfaces
{
    public interface IMediaPlaybackService
    {
        public abstract void NextMediaItem();
        public abstract Task PlayAsync(MediaItem mediaItem);
    }
}
