using System;
using System.Net.Http;
using System.Threading;

namespace Cuture.Http;

/// <summary>
/// Http请求执行状态
/// </summary>
public struct HttpRequestExecuteState : IDisposable
{
    #region Private 字段

    private readonly IOwner<HttpMessageInvoker>? _invokerOwner;
    private int _disposeState;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc cref="HttpResponseMessage"/>
    public HttpResponseMessage HttpResponseMessage { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="HttpRequestExecuteState"/>
    public HttpRequestExecuteState(HttpResponseMessage httpResponseMessage) : this(httpResponseMessage, null)
    {
    }

    /// <inheritdoc cref="HttpRequestExecuteState"/>
    public HttpRequestExecuteState(HttpResponseMessage httpResponseMessage, IOwner<HttpMessageInvoker>? invokerOwner)
    {
        HttpResponseMessage = httpResponseMessage;
        _invokerOwner = invokerOwner;
        _disposeState = 0;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator HttpResponseMessage(HttpRequestExecuteState value) => value.HttpResponseMessage;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _disposeState, 1, 0) == 0)
        {
            _invokerOwner?.Dispose();
            HttpResponseMessage.Dispose();
        }
    }

    #endregion Public 方法
}
