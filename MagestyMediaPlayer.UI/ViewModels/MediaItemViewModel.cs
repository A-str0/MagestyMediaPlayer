using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagestyMediaPlayer.Core.Models;

namespace MagestyMediaPlayer.UI.ViewModels
{
    public class MediaItemViewModel : ViewModelBase
    {
        public string Title => _mediaItem.Title;
        public string Artist => _mediaItem.Title;

        private MediaItem _mediaItem;
        public MediaItem MediaItem => _mediaItem;

        public event Action Test;

        public MediaItemViewModel(MediaItem mediaItem)
        {
            _mediaItem = mediaItem;
        }
    }
}
