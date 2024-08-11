namespace Cuture.Http;

/// <summary>
/// http请求执行选项
/// </summary>
public class HttpRequestExecutionOptions
{
    #region Internal 字段

    internal IJsonSerializer? _jsonSerializer;
    internal IHttpMessageInvokerPool? _messageInvokerPool;

    #endregion Internal 字段

    #region Public 属性

    /// <summary>
    /// 全局使用的默认请求执行选项
    /// </summary>
    public static HttpRequestExecutionOptions Default { get; } = new HttpRequestExecutionOptions()
    {
        MessageInvokerPool = HttpRequestGlobalOptions.DefaultHttpMessageInvokerPool,
        JsonSerializer = HttpRequestGlobalOptions.DefaultJsonSerializer,
    };

    /// <summary>
    /// Json序列化器
    /// </summary>
    public IJsonSerializer? JsonSerializer { get => _jsonSerializer; set => _jsonSerializer = value; }

    /// <summary>
    /// 用于请求的 <see cref="HttpMessageInvoker"/>
    /// <para/>
    /// 设置此选项将覆盖自动重定向、代理请求、压缩、Cookie等请求设置
    /// <para/>
    /// 选项优先级
    /// <para/>
    /// <see cref="MessageInvoker"/> > <see cref="MessageInvokerPool"/>
    /// </summary>
    public HttpMessageInvoker? MessageInvoker { get; set; }

    /// <summary>
    /// 用于请求的 <see cref="IHttpMessageInvokerPool"/>
    /// <para/>
    /// 选项优先级
    /// <para/>
    /// 设置此选项将覆盖自动重定向、代理请求、压缩、Cookie等请求设置
    /// <para/>
    /// <see cref="MessageInvoker"/> > <see cref="MessageInvokerPool"/>
    /// </summary>
    public IHttpMessageInvokerPool? MessageInvokerPool { get => _messageInvokerPool; set => _messageInvokerPool = value; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 获取一份浅表复制
    /// </summary>
    /// <returns></returns>
    public HttpRequestExecutionOptions Clone() => (MemberwiseClone() as HttpRequestExecutionOptions)!;

    #endregion Public 方法
}
