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
        private readonly IMediaPlaybackService _mediaPlaybackService;
        private readonly LocalMediaRepository _localMediaRepository;

        private ObservableCollection<MediaItemViewModel> _items = new ObservableCollection<MediaItemViewModel>();
        public ObservableCollection<MediaItemViewModel> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public ReactiveCommand<MediaItemViewModel, Unit> PlayCommand;

        public LibraryViewModel()
        {
            IServiceProvider serviceProvider = Program.Services.CreateScope().ServiceProvider;

            _mediaPlaybackService = serviceProvider.GetRequiredService<IMediaPlaybackService>();
            _localMediaRepository = serviceProvider.GetRequiredService<IMediaRepository>() as LocalMediaRepository;

            PlayCommand = ReactiveCommand.CreateFromTask<MediaItemViewModel>(PlayAsync);

            Task.Run(async () => await LoadLibraryAsync());
        }

        public async Task PlayAsync(MediaItemViewModel mediaItemViewModel) => await _mediaPlaybackService.PlayAsync(mediaItemViewModel.MediaItem);
    
        private async Task LoadLibraryAsync()
        {
            Debug.WriteLine("Loading library...");

            foreach (var file in _localMediaRepository.SearchLocalFiles("", "101"))
            {
                MediaItem mediaItem = _localMediaRepository.CreateMediaItemFromFile(file);
                await _localMediaRepository.AddMediaItemAsync(mediaItem);
            }

            foreach (var item in await _localMediaRepository.GetAllAsync())
            {
                Debug.WriteLine($"MediaItem {item.Title} loaded");
                Items.Add(new MediaItemViewModel(item));
            }

            Debug.WriteLine("Library loaded");
        }
    }
}
