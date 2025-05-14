using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using MagestyMediaPlayer.Core.Interfaces;
using ReactiveUI;

namespace MagestyMediaPlayer.UI.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        IMediaPlaybackService _mediaPlaybackService;

        public ObservableCollection<MediaItemViewModel> Items { get; } = new();

        public ReactiveCommand<MediaItemViewModel, Unit> PlayCommand;

        public LibraryViewModel(IMediaPlaybackService mediaPlaybackService)
        {
            _mediaPlaybackService = mediaPlaybackService;

            PlayCommand = ReactiveCommand.CreateFromTask<MediaItemViewModel>(PlayAsync);
        }

        public async Task PlayAsync(MediaItemViewModel mediaItemViewModel) => await _mediaPlaybackService.PlayAsync(mediaItemViewModel.MediaItem);
    
        private async Task LoadLibrary()
        {
            //TODO
        }
    }
}
