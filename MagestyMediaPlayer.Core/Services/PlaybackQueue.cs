using MagestyMediaPlayer.Core.Interfaces;

namespace MagestyMediaPlayer.Core.Services
{
    public class PlaybackQueue<T> : IDisposable
    {
        private readonly LinkedList<T> _container = new LinkedList<T>();
        private LinkedListNode<T>? _currentItem;

        public event EventHandler<PlaybackQueueChangedEventArgs<T>>? Changed;

        public T? CurrentItem
        {
            get
            {
                if (_currentItem != null)
                    return _currentItem.Value; // TODO: maybe ValueRef???

                return default;
            }
        }

        public PlaybackQueue() { }

        public void Initialize(IEnumerable<T> items, bool shuffle = false)
        {
            _container.Clear();
            _currentItem = null;

            foreach (T item in items)
                _container.AddLast(item);

            if (shuffle)
                Shuffle();

            _currentItem = _container.First;
        }

        public void AddLast(T item)
        {
            _container.AddLast(item);
            Changed?.Invoke(this, new PlaybackQueueChangedEventArgs<T>() { Value = item });
        }

        public void AddNext(T item)
        {
            if (_currentItem == null)
            {
                Console.WriteLine($"{this}: Current Item Node is null");

                AddLast(item);

                return;
            }

            _container.AddAfter(_currentItem, item);
            Changed?.Invoke(this, new PlaybackQueueChangedEventArgs<T>() { Value = item });
        }


        public void ToNext()
        {
            if (_currentItem == null)
            {
                Console.WriteLine($"{this}: Current Item Node is null");

                return;
            }

            _currentItem = _currentItem.Next;
            Changed?.Invoke(this, new PlaybackQueueChangedEventArgs<T>() { Value = _currentItem.Value });
        }

        public void Shuffle()
        {
            if (_container.Count <= 1)
                return;

            var random = new Random();
            var items = _container.ToList();
            _container.Clear();
            while (items.Count > 0)
            {
                int index = random.Next(items.Count);
                _container.AddLast(items[index]);
                items.RemoveAt(index);
            }

            _currentItem = _container.First;
            Changed?.Invoke(this, new PlaybackQueueChangedEventArgs<T>() { Value = _currentItem.Value });
        }

        public IEnumerable<T> GetAllItems() => _container.ToList();

        public void Dispose()
        {
            _container.Clear();
            _currentItem = null;
            Changed = null;
        }
    }
    
    public class PlaybackQueueChangedEventArgs<T> : EventArgs
    {
        public T? Value { get; set; }
    }
}