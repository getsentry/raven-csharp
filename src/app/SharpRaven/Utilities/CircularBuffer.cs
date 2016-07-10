using System.Collections.Generic;
using System.Linq;

namespace SharpRaven.Utilities {
    public class CircularBuffer<T>
    {
        private readonly int size;
        private readonly Queue<T> queue;

        public CircularBuffer(int size = 100)
        {
            this.size = size;
            queue = new Queue<T>();
        }

        public List<T> ToList()
        {
            return queue.ToList();
        }

        public void Clear()
        {
            queue.Clear();
        }

        public void Add(T item) {
            if (queue.Count >= size)
                queue.Dequeue();

            queue.Enqueue(item);
        }
        
        public bool IsEmpty()
        {
            return queue.Count <= 0;
        }
    }
}
