using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Cuture.Http
{
    /// <summary>
    /// 请求选项
    /// </summary>
    public class HttpRequestOptions
    {
        #region DefaultSetting

        /// <summary>
        /// 默认下载时的buffer大小
        /// </summary>
        public const int DefaultDownloadBufferSize = 10240;

        /// <summary>
        /// 默认的自动循环处理的最大重定向次数
        /// </summary>
        public const int DefaultMaxAutomaticRedirections = 50;

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
        private static IJsonSerializer s_defaultJsonSerializer = new DefaultJsonSerializer();

        /// <summary>
        /// 自动重定向次数
        /// </summary>
        private static int s_maxAutomaticRedirections = DefaultMaxAutomaticRedirections;

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

        /// <summary>
        /// 默认Http头
        /// </summary>
        public static IDictionary<string, string> DefaultHttpHeaders { get; } = new Dictionary<string, string>();

        /// <inheritdoc cref="s_defaultHttpMessageInvokerFactory"/>
        public static IHttpMessageInvokerFactory DefaultHttpMessageInvokerFactory
        {
            get => s_defaultHttpMessageInvokerFactory;
            set
            {
                if (value is null)
                {
                    throw new NullReferenceException($"{nameof(DefaultHttpMessageInvokerFactory)} can not be null");
                }
                if (ReferenceEquals(value, s_defaultHttpMessageInvokerFactory))
                {
                    return;
                }
                var oldFactory = s_defaultHttpMessageInvokerFactory;

                if (ReferenceEquals(Default.MessageInvokerFactory, oldFactory))
                {
                    Default.MessageInvokerFactory = value;
                }

                s_defaultHttpMessageInvokerFactory = value;
                oldFactory.Dispose();
            }
        }

        /// <inheritdoc cref="s_defaultHttpRequestCreator"/>
        public static IHttpRequestCreator DefaultHttpRequestCreator
        {
            get => s_defaultHttpRequestCreator;
            set
            {
                if (value is null)
                {
                    throw new NullReferenceException($"{nameof(DefaultHttpRequestCreator)} can not be null");
                }
                if (ReferenceEquals(value, s_defaultHttpRequestCreator))
                {
                    return;
                }
                var oldFactory = s_defaultHttpRequestCreator;
                s_defaultHttpRequestCreator = value;
                oldFactory.Dispose();
            }
        }

        /// <inheritdoc cref="s_defaultJsonSerializer"/>
        public static IJsonSerializer DefaultJsonSerializer
        {
            get => s_defaultJsonSerializer;
            set
            {
                if (value is null)
                {
                    throw new NullReferenceException($"{nameof(DefaultJsonSerializer)} can not be null");
                }
                if (ReferenceEquals(value, s_defaultJsonSerializer))
                {
                    return;
                }

                if (ReferenceEquals(Default.JsonSerializer, s_defaultJsonSerializer))
                {
                    Default.JsonSerializer = value;
                }

                s_defaultJsonSerializer = value;
            }
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

        #endregion DefaultSetting

        #region Public 属性

        /// <summary>
        /// Json序列化器
        /// </summary>
        public IJsonSerializer? JsonSerializer { get; set; }

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
        public IHttpMessageInvokerFactory? MessageInvokerFactory { get; set; }

        #endregion Public 属性

        #region Public 方法

        /// <summary>
        /// 获取一份浅表复制
        /// </summary>
        /// <returns></returns>
        public HttpRequestOptions Copy() => (MemberwiseClone() as HttpRequestOptions)!;

        #endregion Public 方法
    }
}