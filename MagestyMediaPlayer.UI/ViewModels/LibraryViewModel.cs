using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ZLinq;
using System.Reactive;
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
        private readonly IPlaylistRepository _playlistRepository;

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
            _playlistRepository = serviceProvider.GetRequiredService<IPlaylistRepository>();

            PlayCommand = ReactiveCommand.CreateFromTask<MediaItemViewModel>(PlayAsync);

            Task.Run(LoadLibraryAsync);
        }

        public async Task PlayAsync(MediaItemViewModel mediaItemViewModel)
        {
            _mediaPlaybackService.InitializeQueue(_items.AsValueEnumerable().Select(i => i.MediaItem).ToList(), mediaItemViewModel.MediaItem);

            await _mediaPlaybackService.PlayAsync(_mediaPlaybackService.CurrentQueueItem);
        }
    
        private async Task LoadLibraryAsync()
        {
            Debug.WriteLine($"{this}: Loading library...", "debug");

            // TODO: delete cast
            foreach (var file in (_mediaRepository as LocalMediaRepository).SearchLocalFiles("", "101"))
            {
                MediaItem mediaItem = (_mediaRepository as LocalMediaRepository).CreateMediaItemFromFile(file);
                await _mediaRepository.AddMediaItemAsync(mediaItem);
            }

            // TODO: move this
            Playlist? playlist = (await _playlistRepository.GetAllPlaylistsAsync())
                .AsValueEnumerable()
                .FirstOrDefault(p => p.Name == "Library");
            IEnumerable<Guid> mediaItemIds = (await _mediaRepository.GetAllAsync())
                .AsValueEnumerable()
                .Select(item => item.Id)
                .ToList();

            Debug.WriteLine($"{this}: Created IEnumerable<Guid> {mediaItemIds}", "debug");

            if (playlist == null)
                playlist = await _playlistRepository.CreatePlaylistAsync("Library", "This is library");
            await _playlistRepository.AddTracksToPlaylistAsync(playlist.Id, mediaItemIds);

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
