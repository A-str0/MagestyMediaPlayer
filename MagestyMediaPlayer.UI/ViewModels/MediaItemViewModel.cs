using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using MagestyMediaPlayer.Core.Models;
using ReactiveUI;

namespace MagestyMediaPlayer.UI.ViewModels
{
    public class MediaItemViewModel : ViewModelBase
    {
        public string Title => _mediaItem.Title;
        public string Artist => _mediaItem.Artist ?? "";

        private readonly MediaItem _mediaItem;
        public MediaItem MediaItem => _mediaItem;

        public MediaItemViewModel(MediaItem mediaItem)
        {
            _mediaItem = mediaItem;
        }
    }
}
