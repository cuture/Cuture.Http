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
        /// 默认HttpTurbo构造器
        /// </summary>
        private static IHttpTurboClientFactory s_defaultTurboClientFactory = new SimpleHttpTurboClientFactory();

        /// <summary>
        /// 默认Request工厂
        /// </summary>
        private static IHttpTurboRequestFactory s_defaultTurboRequestFactory = new DefaultRequestFactory();

        /// <summary>
        /// 自动重定向次数
        /// </summary>
        private static int s_maxAutomaticRedirections = DefaultMaxAutomaticRedirections;

        /// <summary>
        /// 默认请求选项
        /// </summary>
        public static HttpRequestOptions Default { get; } = new HttpRequestOptions()
        {
            TurboClientFactory = DefaultTurboClientFactory,
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

        /// <summary>
        /// 默认HttpTurbo构造器
        /// </summary>
        public static IHttpTurboClientFactory DefaultTurboClientFactory
        {
            get => s_defaultTurboClientFactory;
            set
            {
                if (value is null)
                {
                    throw new NullReferenceException($"{nameof(DefaultTurboClientFactory)} can not be null");
                }
                if (ReferenceEquals(value, s_defaultTurboClientFactory))
                {
                    return;
                }
                var oldFactory = s_defaultTurboClientFactory;
                s_defaultTurboClientFactory = value;
                oldFactory.Dispose();
            }
        }

        /// <summary>
        /// 默认Request工厂
        /// </summary>
        public static IHttpTurboRequestFactory DefaultTurboRequestFactory
        {
            get => s_defaultTurboRequestFactory;
            set
            {
                if (value is null)
                {
                    throw new NullReferenceException($"{nameof(DefaultTurboRequestFactory)} can not be null");
                }
                if (ReferenceEquals(value, s_defaultTurboRequestFactory))
                {
                    return;
                }
                var oldFactory = s_defaultTurboRequestFactory;
                s_defaultTurboRequestFactory = value;
                oldFactory.Dispose();
            }
        }

        /// <summary>
        /// 禁止默认使用默认Proxy 初始值为 false
        /// <para/>
        /// 仅对 <see cref="IHttpTurboClientFactory"/> 使用 <see cref="SimpleHttpTurboClientFactory"/>,
        /// 请求为 <see cref="HttpTurboRequest"/> 时有效
        /// <para/>
        /// 不满足上述条件时, 根据对应的具体实现来确定是否有效
        /// </summary>
        public static bool DisableUseDefaultProxyByDefault { get; set; } = false;

        /// <summary>
        /// 自动循环处理的最大重定向次数
        /// </summary>
        public static int MaxAutomaticRedirections { get => s_maxAutomaticRedirections; set => s_maxAutomaticRedirections = value > 0 ? value : throw new ArgumentOutOfRangeException($"{nameof(MaxAutomaticRedirections)} Must be greater than 0"); }

        #endregion DefaultSetting

        #region Public 属性

        /// <summary>
        /// 用于请求的 <see cref="HttpClient"/>
        /// <para/>
        /// 选项优先级
        /// <para/>
        /// 设置此选项将覆盖自动重定向、代理请求、压缩、Cookie等请求设置
        /// <para/>
        /// <see cref="Client"/> > <see cref="TurboClient"/> > <see cref="TurboClientFactory"/>
        /// </summary>
        public HttpClient Client { get; set; }

        /// <summary>
        /// 用于请求的 <see cref="IHttpTurboClient"/>
        /// <para/>
        /// 选项优先级
        /// <para/>
        /// 设置此选项将覆盖自动重定向、代理请求、压缩、Cookie等请求设置
        /// <para/>
        /// <see cref="Client"/> > <see cref="TurboClient"/> > <see cref="TurboClientFactory"/>
        /// </summary>
        public IHttpTurboClient TurboClient { get; set; }

        /// <summary>
        /// 用于请求的 <see cref="IHttpTurboClientFactory"/>
        /// <para/>
        /// 选项优先级
        /// <para/>
        /// 设置此选项将覆盖自动重定向、代理请求、压缩、Cookie等请求设置
        /// <para/>
        /// <see cref="Client"/> > <see cref="TurboClient"/> > <see cref="TurboClientFactory"/>
        /// </summary>
        public IHttpTurboClientFactory TurboClientFactory { get; set; }

        #endregion Public 属性

        #region Public 方法

        /// <summary>
        /// 获取一份浅表复制
        /// </summary>
        /// <returns></returns>
        public HttpRequestOptions Copy() => MemberwiseClone() as HttpRequestOptions;

        #endregion Public 方法
    }
}