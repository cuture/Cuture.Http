using System.Net;
using System.Runtime.CompilerServices;

namespace Cuture.Http;

/// <summary>
/// http请求默认全局选项
/// </summary>
public static class HttpRequestGlobalOptions
{
    #region CONST

    /// <summary>
    /// 默认下载时的buffer大小
    /// </summary>
    public const int DefaultDownloadBufferSize = 10240;

    /// <summary>
    /// 默认的自动循环处理的最大重定向次数
    /// </summary>
    public const int DefaultMaxAutomaticRedirections = 50;

    #endregion CONST

    #region DefaultSetting

    #region Static Fields

    /// <summary>
    /// 全局使用的默认<inheritdoc cref="IFormDataFormatter"/>
    /// </summary>
    private static IFormDataFormatter s_defaultFormDataFormatter;

    /// <summary>
    /// 全局使用的默认<inheritdoc cref="IHttpMessageInvokerPool"/>
    /// </summary>
    private static IHttpMessageInvokerPool s_defaultHttpMessageInvokerPool = new SimpleHttpMessageInvokerPool();

    /// <summary>
    /// 全局使用的默认<inheritdoc cref="IHttpRequestCreator"/>
    /// </summary>
    private static IHttpRequestCreator s_defaultHttpRequestCreator = new DefaultHttpRequestCreator();

    /// <summary>
    /// 全局使用的默认<inheritdoc cref="IJsonSerializer"/>
    /// </summary>
    private static IJsonSerializer s_defaultJsonSerializer = null!;

    /// <summary>
    /// 自动重定向次数
    /// </summary>
    private static int s_maxAutomaticRedirections = DefaultMaxAutomaticRedirections;

    #endregion Static Fields

    #region Static Properties

    /// <summary>
    /// 获取或设置 <see cref="ServicePoint"/> 对象所允许的最大并发连接数。
    /// 也可直接设置 <see cref="ServicePointManager.DefaultConnectionLimit"/>
    /// </summary>
    [Obsolete("参考 System.Net.ServicePointManager")]
    public static int DefaultConnectionLimit
    {
        get => ServicePointManager.DefaultConnectionLimit;
        set => ServicePointManager.DefaultConnectionLimit = value;
    }

    /// <inheritdoc cref="s_defaultFormDataFormatter"/>
    public static IFormDataFormatter DefaultFormDataFormatter
    {
        get => s_defaultFormDataFormatter;
        set => SetOption(ref s_defaultFormDataFormatter, ref s_defaultFormDataFormatter!, value);
    }

    /// <summary>
    /// 默认Http头
    /// </summary>
    public static IDictionary<string, string> DefaultHttpHeaders { get; } = new Dictionary<string, string>();

    /// <inheritdoc cref="s_defaultHttpMessageInvokerPool"/>
    public static IHttpMessageInvokerPool DefaultHttpMessageInvokerPool
    {
        get => s_defaultHttpMessageInvokerPool;
        set => SetOption(ref s_defaultHttpMessageInvokerPool, ref HttpRequestExecutionOptions.Default._messageInvokerPool, value);
    }

    /// <inheritdoc cref="s_defaultHttpRequestCreator"/>
    public static IHttpRequestCreator DefaultHttpRequestCreator
    {
        get => s_defaultHttpRequestCreator;
        set => SetOption(ref s_defaultHttpRequestCreator, ref s_defaultHttpRequestCreator!, value);
    }

    /// <inheritdoc cref="s_defaultJsonSerializer"/>
    public static IJsonSerializer DefaultJsonSerializer
    {
        get => s_defaultJsonSerializer;
        set => SetOption(ref s_defaultJsonSerializer, ref HttpRequestExecutionOptions.Default._jsonSerializer, value);
    }

    /// <summary>
    /// 禁止默认使用默认Proxy 初始值为 false
    /// <para/>
    /// 仅对 <see cref="IHttpMessageInvokerPool"/> 使用 <see cref="IHttpMessageInvokerPool"/>,
    /// 请求为 <see cref="DefaultHttpRequest"/> 时有效
    /// <para/>
    /// 不满足上述条件时, 根据对应的具体实现来确定是否有效
    /// </summary>
    public static bool DisableUseDefaultProxyByDefault { get; set; }

    /// <summary>
    /// 自动循环处理的最大重定向次数
    /// </summary>
    public static int MaxAutomaticRedirections { get => s_maxAutomaticRedirections; set => s_maxAutomaticRedirections = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(MaxAutomaticRedirections), "Must be greater than 0"); }

    #endregion Static Properties

    #endregion DefaultSetting

    #region 静态构造函数

    /// <inheritdoc/>
    static HttpRequestGlobalOptions()
    {
        s_defaultJsonSerializer = new SystemJsonJsonSerializer();
        s_defaultFormDataFormatter = new SystemJsonFormDataFormatter();
    }

    #endregion 静态构造函数

    #region Private 方法

    private static void SetOption<T>(ref T target, ref T? compareTarget, T value, [CallerMemberName] string? propertyName = null)
    {
        if (value is null)
        {
            throw new NullReferenceException($"{propertyName} can not be null");
        }

        if (ReferenceEquals(value, target))
        {
            return;
        }

        var oldValue = target;
        target = value;

        if (compareTarget != null && ReferenceEquals(compareTarget, oldValue))
        {
            compareTarget = value;
        }

        (oldValue as IDisposable)?.Dispose();
    }

    #endregion Private 方法
}
