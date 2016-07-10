using System;
using System.Collections.Concurrent;
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
            return queue.Count <= 0;
        }
    }
}
