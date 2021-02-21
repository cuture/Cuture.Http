using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    /// <summary>
    /// 请求选项
    /// </summary>
    public class HttpRequestOptions
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
        /// 全局使用的默认<inheritdoc cref="IHttpMessageInvokerFactory"/>
        /// </summary>
        private static IHttpMessageInvokerFactory s_defaultHttpMessageInvokerFactory = new SimpleHttpMessageInvokerFactory();

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

        #region Fields

        private IJsonSerializer? _jsonSerializer;
        private IHttpMessageInvokerFactory? _messageInvokerFactory;

        #endregion Fields

        #region Static Properties

        /// <summary>
        /// 全局使用的默认请求选项
        /// </summary>
        public static HttpRequestOptions Default { get; } = new HttpRequestOptions()
        {
            MessageInvokerFactory = DefaultHttpMessageInvokerFactory,
            JsonSerializer = DefaultJsonSerializer,
        };

        /// <summary>
        /// 获取或设置 <see cref="ServicePoint"/> 对象所允许的最大并发连接数。
        /// 也可直接设置 <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// </summary>
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

        /// <inheritdoc cref="s_defaultHttpMessageInvokerFactory"/>
        public static IHttpMessageInvokerFactory DefaultHttpMessageInvokerFactory
        {
            get => s_defaultHttpMessageInvokerFactory;
            set => SetOption(ref s_defaultHttpMessageInvokerFactory, ref Default._messageInvokerFactory, value);
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
            set => SetOption(ref s_defaultJsonSerializer, ref Default._jsonSerializer, value);
        }

        /// <summary>
        /// 禁止默认使用默认Proxy 初始值为 false
        /// <para/>
        /// 仅对 <see cref="IHttpMessageInvokerFactory"/> 使用 <see cref="SimpleHttpMessageInvokerFactory"/>,
        /// 请求为 <see cref="DefaultHttpRequest"/> 时有效
        /// <para/>
        /// 不满足上述条件时, 根据对应的具体实现来确定是否有效
        /// </summary>
        public static bool DisableUseDefaultProxyByDefault { get; set; }

        /// <summary>
        /// 自动循环处理的最大重定向次数
        /// </summary>
        public static int MaxAutomaticRedirections { get => s_maxAutomaticRedirections; set => s_maxAutomaticRedirections = value > 0 ? value : throw new ArgumentOutOfRangeException($"{nameof(MaxAutomaticRedirections)} Must be greater than 0"); }

        #endregion Static Properties

        #endregion DefaultSetting

        #region 静态构造函数

        static HttpRequestOptions()
        {
#if NETCOREAPP
            s_defaultJsonSerializer = new SystemJsonJsonSerializer();
            s_defaultFormDataFormatter = new SystemJsonFormDataFormatter();
#endif
#if LEGACYTFM
            s_defaultJsonSerializer = new NewtonsoftJsonJsonSerializer();
            s_defaultFormDataFormatter = new NewtonsoftJsonFormDataFormatter();
#endif
        }

        #endregion 静态构造函数

        #region Public 属性

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
        /// <see cref="MessageInvoker"/> > <see cref="MessageInvokerFactory"/>
        /// </summary>
        public HttpMessageInvoker? MessageInvoker { get; set; }

        /// <summary>
        /// 用于请求的 <see cref="IHttpMessageInvokerFactory"/>
        /// <para/>
        /// 选项优先级
        /// <para/>
        /// 设置此选项将覆盖自动重定向、代理请求、压缩、Cookie等请求设置
        /// <para/>
        /// <see cref="MessageInvoker"/> > <see cref="MessageInvokerFactory"/>
        /// </summary>
        public IHttpMessageInvokerFactory? MessageInvokerFactory { get => _messageInvokerFactory; set => _messageInvokerFactory = value; }

        #endregion Public 属性

        #region Public 方法

        /// <summary>
        /// 获取一份浅表复制
        /// </summary>
        /// <returns></returns>
        public HttpRequestOptions Copy() => (MemberwiseClone() as HttpRequestOptions)!;

        #endregion Public 方法

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
}