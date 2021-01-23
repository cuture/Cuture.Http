using System;

namespace Cuture.Http.Internal
{
    internal class CreatedTimeTagedObject<T> : IComparable<CreatedTimeTagedObject<T>> where T : class
    {
        #region Public 属性

        public DateTime CreatedTime { get; }
        public T Data { get; }

        #endregion Public 属性

        #region Public 构造函数

        public CreatedTimeTagedObject(T data) : this(data, DateTime.UtcNow)
        {
        }

        public CreatedTimeTagedObject(T data, DateTime createdTime)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            CreatedTime = createdTime;
        }

        public int CompareTo(CreatedTimeTagedObject<T>? other)
        {
            return CreatedTime.CompareTo(other?.CreatedTime);
        }

        #endregion Public 构造函数
    }
}