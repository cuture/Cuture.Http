﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.Http;

/// <summary>
/// 序列化器
/// </summary>
/// <typeparam name="TData"></typeparam>
public interface ISerializer<TData>
{
    #region Public 方法

    /// <summary>
    /// 反序列化数据到对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    T? Deserialize<T>(TData data);

    /// <summary>
    /// 从流反序列化数据到对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// 序列化到数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    TData Serialize(object value);

    #endregion Public 方法
}