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
#if net35
        private class ConcurrentQueue<K> : ICollection, IEnumerable<K>
        {
            private readonly Queue<K> _queue;

            public ConcurrentQueue()
            {
                _queue = new Queue<K>();
            }

            public IEnumerator<K> GetEnumerator()
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
                    lock (SyncRoot)
                    {
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

            public bool IsEmpty
            {
                get { return this.Count == 0; }
            }


            public void Enqueue(K item)
            {
                lock (SyncRoot)
                {
                    _queue.Enqueue(item);
                }
            }

            public K Dequeue()
            {
                lock (SyncRoot)
                {
                    return _queue.Dequeue();
                }
            }

            public K Peek()
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


            public void TryDequeue(out K result)
            {
                try
                {
                    result = Dequeue();
                }
                catch
                {
                    result = default(K);
                }
            }
        }
#endif
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
}
