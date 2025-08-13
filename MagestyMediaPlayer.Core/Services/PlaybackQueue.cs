using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using MagestyMediaPlayer.Core.Models;

namespace MagestyMediaPlayer.Core.Services
{
    public class PlaybackQueue : INotifyCollectionChanged, IDisposable
    {
        private readonly List<MediaItem> _container = new();
        private int _currentIndex = -1;
        private readonly int _maxSize;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event EventHandler<MediaItem?>? CurrentItemChanged;

        public MediaItem? Current => _currentIndex >= 0 && _currentIndex < _container.Count ? _container[_currentIndex] : null;
        public IReadOnlyList<MediaItem> Items => _container.AsReadOnly();
        public bool HasNext => _currentIndex + 1 < _container.Count;
        public bool HasPrevious => _currentIndex > 0;
        public int Count => _container.Count;

        public PlaybackQueue(int maxSize = 500)
        {
            _maxSize = maxSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLast(MediaItem item)
        {
            if (_container.Count >= _maxSize)
            {
                Debug.WriteLine($"{this}: Container size is already maximum {_container.Count}", "debug");
                return;
            }

            _container.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _container.Count - 1));
        }

        public void AddNext(MediaItem item)
        {
            if (_container.Count >= _maxSize)
            {
                Debug.WriteLine($"{this}: Container size is already maximum {_container.Count}", "debug");
                return;
            }

            int insertIndex = Math.Clamp(_currentIndex + 1, 0, _maxSize);
            _container.Insert(insertIndex, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, insertIndex));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveNext()
        {
            if (!HasNext)
            {
                Debug.WriteLine($"{this}: There is no Next Item in container", "debug");
                return;
            }

            _currentIndex++;
            CurrentItemChanged?.Invoke(this, Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MovePrevious()
        {
            if (!HasPrevious)
            {
                Debug.WriteLine($"{this}: There is no Previous Item in container", "debug");
                return;
            }

            _currentIndex--;
            CurrentItemChanged?.Invoke(this, Current);
        }

        public void Initialize(IEnumerable<MediaItem> items, MediaItem? selectedItem = null, bool shuffle = false)
        {
            _container.Clear();

            foreach (var item in items)
            {
                AddLast(item);
            }

            _currentIndex = selectedItem == null ? 0 : _container.IndexOf(selectedItem);
            CurrentItemChanged?.Invoke(this, Current);

            if (shuffle)
                Shuffle();
        }

        public void Shuffle()
        {
            if (_container.Count <= 1)
                return;

            var random = new Random();

            for (int i = _container.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (_container[i], _container[j]) = (_container[j], _container[i]);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, _container[i], i, j));
            }

            if (_currentIndex >= 0)
                _currentIndex = Current != null ? _container.IndexOf(Current) : 0;

            CurrentItemChanged?.Invoke(this, Current);
        }

        public void Clear()
        {
            _container.Clear();
            _currentIndex = -1;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            CurrentItemChanged?.Invoke(this, null);
        }

        // TODO
        // public async Task InitializeAsync(QueueSourceType sourceType, AppDbContext context, Guid? sourceId = null, Guid? selectedId = null, bool shuffle = false)
        // {
        //     var factory = new QueueFactory(context);
        //     var strategy = factory.Create(sourceType, sourceId);
        //     await strategy.GenerateAsync(this, selectedId, shuffle);
        // }

        public void Dispose()
        {
            Clear();

            CollectionChanged = null;
            CurrentItemChanged = null;
        }
    }
}