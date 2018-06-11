using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDA
{
    public class PriorityQueue<T, K> where K : class
    {
        private Dictionary<T, Dictionary<T, Queue<K>>> queue;
        private int count;

        public int Count
        {
            get
            {
                return count;
            }

            set
            {
                count = value;
            }
        }

        public PriorityQueue()
        {
            queue = new Dictionary<T, Dictionary<T, Queue<K>>>();
            count = 0;
        }

        public void Enqueue(T key1, T key2, K v)
        {
            if (!queue.ContainsKey(key1))
                queue.Add(key1, new Dictionary<T, Queue<K>>());
            if (!queue[key1].ContainsKey(key2))
                queue[key1].Add(key2, new Queue<K>());
            queue[key1][key2].Enqueue(v);
            count++;
        }

        public K Dequeue()
        {
            if (queue.Keys.Count == 0)
                return null;
            T minKey1 = queue.Keys.Min();
            T minKey2 = queue[minKey1].Keys.Min();
            K v = queue[minKey1][minKey2].Dequeue();
            if (queue[minKey1][minKey2].Count == 0)
                queue[minKey1].Remove(minKey2);
            if (queue[minKey1].Count == 0)
                queue.Remove(minKey1);
            count--;
            return v;
        }

    }
}
