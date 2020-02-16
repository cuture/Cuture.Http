using System;
using System.Collections.Generic;
using System.Net;

namespace Cuture.Http
{
    /// <summary>
    /// Http相关的默认设置
    /// </summary>
    public static class HttpDefaultSetting
    {
        #region 字段

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

        private static int s_maxAutomaticRedirections = DefaultMaxAutomaticRedirections;

        #endregion 字段

        #region 属性

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
        public static Dictionary<string, string> DefaultHttpHeaders { get; } = new Dictionary<string, string>();

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

        #endregion 属性
    }
}