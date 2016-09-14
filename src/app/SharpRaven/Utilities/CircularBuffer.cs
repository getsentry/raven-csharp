using System;
#if !(net35)
using System.Collections.Concurrent;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpRaven.Utilities {
    public class CircularBuffer<T>
    {
        private readonly int size;
        private ConcurrentQueue<T> queue;

        public CircularBuffer(int size = 100)
        {
            this.size = size;
            queue = new ConcurrentQueue<T>();
        }

        public List<T> ToList()
        {
            var listReturn = this.queue.ToList();
            
            return listReturn.Skip(Math.Max(0, listReturn.Count - size)).ToList();
        }

        public void Clear()
        {
            queue = new ConcurrentQueue<T>();
        }

        public void Add(T item) {
            if (queue.Count >= size)
            {
                T result;
                this.queue.TryDequeue(out result);
            }

            queue.Enqueue(item);
        }
        
        public bool IsEmpty()
        {
            return queue.IsEmpty;
        }
    }

    internal class ConcurrentQueue<T> : ICollection, IEnumerable<T>
    {
        private readonly Queue<T> _queue;

        public ConcurrentQueue()
        {
            _queue = new Queue<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (SyncRoot)
            {
                foreach (var item in _queue)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            lock (SyncRoot)
            {
                ((ICollection)_queue).CopyTo(array, index);
            }
        }

        public int Count
        {
            get
            {
                lock (SyncRoot) {
                    return _queue.Count;
                }
            }
        }

        public object SyncRoot
        {
            get { return ((ICollection)_queue).SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public bool IsEmpty {
            get { return this.Count == 0; }
        }


        public void Enqueue(T item)
        {
            lock (SyncRoot)
            {
                _queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            lock (SyncRoot)
            {
                return _queue.Dequeue();
            }
        }

        public T Peek()
        {
            lock (SyncRoot)
            {
                return _queue.Peek();
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                _queue.Clear();
            }
        }


        public void TryDequeue(out T result) {
            try {
                result = Dequeue();
            }
            catch {
                result = default(T);
            }
        }
    }
}
