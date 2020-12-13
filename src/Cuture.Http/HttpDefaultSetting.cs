using System;
using System.Collections.Generic;
using System.Net;

namespace Cuture.Http
{
    /// <summary>
    /// Http相关的默认设置
    /// </summary>
    [Obsolete("使用 HttpRequestOptions 替换", true)]
    public static class HttpDefaultSetting
    {
        #region 字段

        /// <summary>
        /// 默认下载时的buffer大小
        /// </summary>
        public const int DefaultDownloadBufferSize = HttpRequestOptions.DefaultDownloadBufferSize;

        /// <summary>
        /// 默认的自动循环处理的最大重定向次数
        /// </summary>
        public const int DefaultMaxAutomaticRedirections = HttpRequestOptions.DefaultMaxAutomaticRedirections;

        #endregion 字段

        #region 属性

        /// <summary>
        /// 获取或设置 <see cref="ServicePoint"/> 对象所允许的最大并发连接数。
        /// 也可直接设置 <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// </summary>
        public static int DefaultConnectionLimit
        {
            get => HttpRequestOptions.DefaultConnectionLimit;
            set => HttpRequestOptions.DefaultConnectionLimit = value;
        }

        /// <summary>
        /// 默认HttpTurbo构造器
        /// </summary>
        public static IHttpTurboClientFactory DefaultTurboClientFactory
        {
            get => HttpRequestOptions.DefaultTurboClientFactory;
            set => HttpRequestOptions.DefaultTurboClientFactory = value;
        }

        /// <summary>
        /// 默认Request工厂
        /// </summary>
        public static IHttpTurboRequestFactory DefaultTurboRequestFactory
        {
            get => HttpRequestOptions.DefaultTurboRequestFactory;
            set => HttpRequestOptions.DefaultTurboRequestFactory = value;
        }

        /// <summary>
        /// 禁止默认使用默认Proxy 初始值为 false
        /// <para/>
        /// 仅对 <see cref="IHttpTurboClientFactory"/> 使用 <see cref="SimpleHttpTurboClientFactory"/>,
        /// 请求为 <see cref="HttpTurboRequest"/> 时有效
        /// <para/>
        /// 不满足上述条件时, 根据对应的具体实现来确定是否有效
        /// </summary>
        public static bool DisableUseDefaultProxyByDefault { get => HttpRequestOptions.DisableUseDefaultProxyByDefault; set => HttpRequestOptions.DisableUseDefaultProxyByDefault = value; }

        /// <summary>
        /// 自动循环处理的最大重定向次数
        /// </summary>
        public static int MaxAutomaticRedirections { get => HttpRequestOptions.MaxAutomaticRedirections; set => HttpRequestOptions.MaxAutomaticRedirections = value; }

        /// <summary>
        /// 默认Http头
        /// </summary>
        public static IDictionary<string, string> DefaultHttpHeaders => HttpRequestOptions.DefaultHttpHeaders;

        #endregion 属性
    }
}