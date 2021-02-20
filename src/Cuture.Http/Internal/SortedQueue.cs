namespace System.Collections.Generic
{
    /// <summary>
    /// 线程不安全的有序队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SortedQueue<T> : IEnumerable<T>
    {
        #region Private 字段

        private readonly SortedSet<T> _set;

        #endregion Private 字段

        #region Public 属性

        public int Count => _set.Count;

        #endregion Public 属性

        #region Public 构造函数

        public SortedQueue() : this(null, null)
        {
        }

        public SortedQueue(IComparer<T>? comparer) : this(null, comparer)
        {
        }

        public SortedQueue(IEnumerable<T>? collection) : this(collection, null)
        {
        }

        public SortedQueue(IEnumerable<T>? collection, IComparer<T>? comparer)
        {
            _set = collection is null
                    ? comparer is null
                        ? new SortedSet<T>()
                        : new SortedSet<T>(comparer)
                    : comparer is null
                        ? new SortedSet<T>(collection)
                        : new SortedSet<T>(collection, comparer);
        }

        #endregion Public 构造函数

        #region Public 方法

        public void Clear()
        {
            _set.Clear();
        }

        public T? Dequeue()
        {
            var result = _set.Min;
            if (result != null)
            {
                _set.Remove(result);
            }
            return result;
        }

        public bool Enqueue(T item) => _set.Add(item);

        public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();

        public T? Peek() => _set.Min;

        IEnumerator IEnumerable.GetEnumerator() => _set.GetEnumerator();

        #endregion Public 方法
    }
}