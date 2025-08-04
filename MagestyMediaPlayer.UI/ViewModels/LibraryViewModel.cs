using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using MagestyMediaPlayer.Core.Interfaces;
using MagestyMediaPlayer.Core.Models;
using MagestyMediaPlayer.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace MagestyMediaPlayer.UI.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        private readonly MediaPlaybackService _mediaPlaybackService;
        private readonly IMediaRepository _mediaRepository;

        private ObservableCollection<MediaItemViewModel> _items = new ObservableCollection<MediaItemViewModel>();
        public ObservableCollection<MediaItemViewModel> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        private MediaItemViewModel? _selectedMediaItem;
        public MediaItemViewModel? SelectedMediaItem
        {
            get => _selectedMediaItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedMediaItem, value);
                if (_selectedMediaItem != null)
                {
                    PlayCommand.Execute(_selectedMediaItem).Subscribe();
                }
            }
        }

        public ReactiveCommand<MediaItemViewModel, Unit> PlayCommand;

        public LibraryViewModel()
        {
            IServiceProvider serviceProvider = Program.Services.CreateScope().ServiceProvider;

            _mediaPlaybackService = serviceProvider.GetRequiredService<MediaPlaybackService>();
            _mediaRepository = serviceProvider.GetRequiredService<IMediaRepository>();

            PlayCommand = ReactiveCommand.CreateFromTask<MediaItemViewModel>(PlayAsync);

            Task.Run(LoadLibraryAsync);
        }

        public async Task PlayAsync(MediaItemViewModel mediaItemViewModel) => await _mediaPlaybackService.PlayAsync(mediaItemViewModel.MediaItem);
    
        private async Task LoadLibraryAsync()
        {
            Debug.WriteLine("Loading library...");

            // TODO: delete cast
            foreach (var file in (_mediaRepository as LocalMediaRepository).SearchLocalFiles("", "101"))
            {

                MediaItem mediaItem = (_mediaRepository as LocalMediaRepository).CreateMediaItemFromFile(file);

                await _mediaRepository.AddMediaItemAsync(mediaItem);
            }

            foreach (var item in await _mediaRepository.GetAllAsync())
            {
                if (item == null)
                    continue;

                Debug.WriteLine($"{this}: MediaItem {item.Title} loaded", "debug");
                Items.Add(new MediaItemViewModel(item));
            }

            Debug.WriteLine($"{this}: Library loaded", "debug");
        }
    }
}
