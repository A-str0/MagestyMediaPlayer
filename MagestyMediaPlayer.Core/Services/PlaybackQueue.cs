using MagestyMediaPlayer.Core.Interfaces;

namespace MagestyMediaPlayer.Core.Services
{
    public class PlaybackQueue<T> : IDisposable
    {
        private LinkedList<T> _containter;

        private LinkedListNode<T>? _currentItem;

        public event EventHandler<PlaybackQueueChangedEventArgs<T>>? Changed;

        public T CurrentItem => _currentItem.Value; // TODO: maybe ValueRef???

        public PlaybackQueue()
        {
            _containter = new LinkedList<T>();
        }

        public void AddLast(T item)
        {
            _containter.AddLast(item);
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

            _containter.AddAfter(_currentItem, item);
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

        public void Dispose()
        {
            _containter.Clear();
        }
    }
    
    public class PlaybackQueueChangedEventArgs<T> : EventArgs
    {
        public T Value { get; set; }
    }
}