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
        /// 默认HttpTurbo构造器
        /// </summary>
        private static IHttpTurboClientFactory s_defaultTurboClientFactory = new SimpleHttpTurboClientFactory();

        /// <summary>
        /// 获取或设置 <see cref="ServicePoint"/> 对象所允许的最大并发连接数。
        /// 或直接设置 <see cref="ServicePointManager"/>
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

        #endregion 字段
    }
}