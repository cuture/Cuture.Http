using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS8766 // 返回类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。
#pragma warning disable CS8767 // 参数类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。

namespace Cuture.Http;

/// <summary>
/// 默认<see cref="IHttpRequest"/> 实现
/// <para/>
/// 派生自 <see cref="HttpRequestMessage"/> 的 <see cref="IHttpRequest"/> 实现
/// <para/>
/// * 请求后会被释放，不可重复使用进行请求
/// </summary>

public class DefaultHttpRequest : HttpRequestMessage, IHttpRequest
{
    #region Private 字段

    private int _hasGetHttpRequestMessage = 0;

    private HttpRequestExecutionOptions? _options;

    #endregion Private 字段

    #region 属性

    /// <inheritdoc/>
    public bool AllowRedirection { get; set; }

    /// <summary>
    /// 禁用Proxy
    /// <para/>初始值为 <see cref="HttpRequestGlobalOptions.DisableUseDefaultProxyByDefault"/>
    /// </summary>
    public bool DisableProxy { get; set; } = HttpRequestGlobalOptions.DisableUseDefaultProxyByDefault;

    /// <inheritdoc/>
    public HttpRequestExecutionOptions ExecutionOptions
    {
        get => _options ??= HttpRequestExecutionOptions.Default.Clone();
        set => _options = value;
    }

    /// <inheritdoc/>
    public bool IsSetOptions => _options != null;

    /// <inheritdoc/>
    public int MaxAutomaticRedirections { get; set; } = HttpRequestGlobalOptions.MaxAutomaticRedirections;

    /// <inheritdoc/>
    public IWebProxy? Proxy { get; set; }

    /// <inheritdoc/>
    public int? Timeout { get; set; }

    /// <inheritdoc/>
    public CancellationToken Token { get; set; }

    /// <inheritdoc/>
    HttpHeaders IHttpRequest.Headers => Headers;

    #endregion 属性

    #region 构造函数

    /// <summary>
    /// <inheritdoc cref="DefaultHttpRequest"/>
    /// </summary>
    /// <param name="requestUri">请求的Uri</param>
    public DefaultHttpRequest(Uri requestUri)
    {
        RequestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
    }

    #endregion 构造函数

    #region 方法

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<HttpRequestMessage> GetHttpRequestMessageAsync(CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref _hasGetHttpRequestMessage, 1, 0) == 1)
        {
            throw new InvalidOperationException("Method GetHttpRequestMessage can only be called once.");
        }
        return new(this);
    }

    #endregion 方法
}
