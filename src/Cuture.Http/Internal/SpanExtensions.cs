#pragma warning disable IDE0130

namespace System;

internal static class SpanExtensions
{
    #region Public 方法

    /// <summary>
    /// 以指定标识符截断数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="identifier"></param>
    /// <param name="removeFront"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> TruncationEnd<T>(this in ReadOnlySpan<T> data, T identifier, bool removeFront = true) where T : IEquatable<T>
    {
        return Slice(data, data.LastIndexOf(identifier), 1, removeFront);
    }

    /// <summary>
    /// 以指定标识符截断数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="identifier"></param>
    /// <param name="removeFront"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> TruncationEnd<T>(this in ReadOnlySpan<T> data, in ReadOnlySpan<T> identifier, bool removeFront = true) where T : IEquatable<T>
    {
        return Slice(data, data.LastIndexOf(identifier), identifier.Length, removeFront);
    }

    /// <summary>
    /// 以指定标识符截断数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="identifier"></param>
    /// <param name="removeFront"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> TruncationStart<T>(this in ReadOnlySpan<T> data, T identifier, bool removeFront = true) where T : IEquatable<T>
    {
        return Slice(data, data.IndexOf(identifier), 1, removeFront);
    }

    /// <summary>
    /// 以指定标识符截断数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="identifier"></param>
    /// <param name="removeFront"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> TruncationStart<T>(this in ReadOnlySpan<T> data, in ReadOnlySpan<T> identifier, bool removeFront = true) where T : IEquatable<T>
    {
        return Slice(data, data.IndexOf(identifier), identifier.Length, removeFront);
    }

    #endregion Public 方法

    #region Private 方法

    private static ReadOnlySpan<T> Slice<T>(in ReadOnlySpan<T> data, int index, int fixLength, bool removeFront = true)
    {
        if (index < 0)
        {
            return data;
        }
        return removeFront
            ? data.Slice(index + fixLength)
            : data.Slice(0, index);
    }

    #endregion Private 方法
}
