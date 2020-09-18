using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cuture.Http
{
    /// <summary>
    /// UserAgent生成集合工具类
    /// </summary>
    public static class UserAgents
    {
        #region 字段

        /// <summary>
        /// Chrome 85
        /// </summary>
        public const string Chrome = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36";

        /// <summary>
        /// Edge
        /// </summary>
        public const string Edge = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.19041";

        /// <summary>
        /// Edge on Chromium 85
        /// </summary>
        public const string EdgeChromium = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36 Edg/85.0.564.51";

        /// <summary>
        /// FireFox 80
        /// </summary>
        public const string FireFox = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0";

        /// <summary>
        /// IE 11
        /// </summary>
        public const string IExplore = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

        /// <summary>
        /// 随机数生成器
        /// </summary>
        private static readonly Random s_random = new Random();

        #endregion 字段

        #region 方法

        #region BrowserVersion

        /// <summary>
        /// 随机Chrome版本
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string RandomChromeVersion()
        {
            return $"Chrome/{s_random.Next(49, 89)}.0.{s_random.Next(3000, 5500)}.{s_random.Next(50, 200)}";
        }

        /// <summary>
        /// 随机的Edge版本
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string RandomEdgeVersion()
        {
            return $"Edge/{s_random.Next(15, 19)}.{s_random.Next(10240, 19200)}";
        }

        #endregion BrowserVersion

        #region Part

        /// <summary>
        /// 随机附加字段
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string RandomAddOn()
        {
            return (s_random.Next(2)) switch
            {
                1 => string.Empty,
                _ => "AppleWebKit/537.36",
            };
        }

        /// <summary>
        /// 随机浏览器
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string RandomBrowser()
        {
            return (s_random.Next(4)) switch
            {
                2 => RandomEdgeVersion(),
                _ => RandomChromeVersion(),
            };
        }

        /// <summary>
        /// 随机火狐平台
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string RandomFirefoxPlatform(int version)
        {
            return (s_random.Next(7)) switch
            {
                1 => $"(Windows NT 10.0; rv:{version}.0)",
                2 => $"(Windows NT 6.1; Win64; x64; rv:{version}.0)",
                3 => $"(Windows NT 6.1; Win64; rv:{version}.0)",
                4 => $"(Windows NT 6.1; rv:{version}.0)",
                5 => $"(Windows NT 10.0; Win64; x64; rv:{version}.0)",
                6 => $"(Windows NT 10.0; Win64; rv:{version}.0)",
                _ => $"(Windows NT 10.0; Win64; x64; rv:{version}.0)",
            };
        }

        /// <summary>
        /// 随机UA尾部附加
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string RandomLast()
        {
            return (s_random.Next(4)) switch
            {
                1 => RandomEdgeVersion(),
                2 => $"Safari/537.36 {RandomEdgeVersion()}",
                _ => "Safari/537.36",
            };
        }

        /// <summary>
        /// 国内魔改UA
        /// </summary>
        /// <returns></returns>
        private static string RandomModified()
        {
            switch (s_random.Next(7))
            {
                case 0: //搜狗
                    return "SE 2.X MetaSr 1.0";

                case 1: //QQ
                    StringBuilder uaBuilderQQ = new StringBuilder("Core/1.");
                    uaBuilderQQ.Append(s_random.Next(61, 74));
                    uaBuilderQQ.Append('.');
                    uaBuilderQQ.Append(s_random.Next(6200, 7600));
                    uaBuilderQQ.Append('.');
                    int qqInternal = s_random.Next(200, 800);
                    uaBuilderQQ.Append(qqInternal);
                    uaBuilderQQ.Append(" QQBrowser/10.");
                    uaBuilderQQ.Append(s_random.Next(1, 5));
                    uaBuilderQQ.Append('.');
                    uaBuilderQQ.Append(s_random.Next(2000, 3500));
                    uaBuilderQQ.Append('.');
                    uaBuilderQQ.Append(qqInternal);

                    return uaBuilderQQ.ToString();    // "1.63.6721.400 QQBrowser/10.2.2243.400";
                case 2: //猎豹
                    return "LBBROWSER";

                case 3: //2345
                    StringBuilder uaBuilder2345 = new StringBuilder("2345Explorer/");
                    uaBuilder2345.Append(s_random.Next(8, 11));
                    uaBuilder2345.Append('.');
                    uaBuilder2345.Append(s_random.Next(1, 5));
                    uaBuilder2345.Append('.');
                    uaBuilder2345.Append(s_random.Next(1, 5));
                    uaBuilder2345.Append('.');
                    uaBuilder2345.Append(s_random.Next(16500, 18200));

                    return uaBuilder2345.ToString();    // "2345Explorer/9.2.1.17116";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 随机平台
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string RandomPlatform()
        {
            return (s_random.Next(7)) switch
            {
                1 => "(Windows NT 10.0)",
                2 => "(Windows NT 6.1; Win64; x64)",
                3 => "(Windows NT 6.1; Win64;)",
                4 => "(Windows NT 6.1)",
                5 => "(Windows NT 10.0; Win64; x64)",
                6 => "(Windows NT 10.0; Win64;)",
                _ => "(Windows NT 10.0; Win64; x64)",
            };
        }

        /// <summary>
        /// 随机核心标记
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string RandomTag()
        {
            return (s_random.Next(3)) switch
            {
                1 => "Gecko/20100101",
                2 => "like Gecko",
                _ => "(KHTML, like Gecko)",
            };
        }

        #endregion Part

        #region FullUserAgent

        /// <summary>
        /// 随机Chrome版本UA
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RandomChromeUserAgent()
        {
            return $"Mozilla/5.0 {RandomPlatform()} AppleWebKit/537.36 (KHTML, like Gecko) {RandomChromeVersion()} Safari/537.36";
        }

        /// <summary>
        /// 随机Firefox版本UA
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RandomFirefoxUserAgent()
        {
            var version = s_random.Next(60, 85);
            return $"Mozilla/5.0 {RandomFirefoxPlatform(version)} Gecko/20100101 Firefox/{version}.0";
        }

        /// <summary>
        /// 随机移动UserAgent
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RandomMobileUserAgent()
        {
            return BuildUserAgent(new string[] {
                RandomPlatform(),
                RandomAddOn(),
                RandomTag(),
                RandomBrowser(),
                RandomLast(),
                RandomModified(),
                "Mobile",
            });
        }

        /// <summary>
        /// 随机国内魔改UserAgent
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RandomModifiedUserAgent()
        {
            return BuildUserAgent(new string[] {
                RandomPlatform(),
                "AppleWebKit/537.36 (KHTML, like Gecko)",
                RandomModified(),
                "Safari/537.36",
                RandomModified(),
            });
        }

        /// <summary>
        /// 随机UserAgent
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RandomUserAgent()
        {
            return BuildUserAgent(new string[] {
                RandomPlatform(),
                RandomAddOn(),
                RandomTag(),
                RandomBrowser(),
                RandomLast(),
                RandomModified(),
            });
        }

        #endregion FullUserAgent

        private static string BuildUserAgent(IEnumerable<string> parts)
        {
            var uaBuilder = new StringBuilder("Mozilla/5.0 ", 512);
            foreach (var item in parts)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    uaBuilder.Append(item);
                    uaBuilder.Append(' ');
                }
            }

            uaBuilder.Remove(uaBuilder.Length - 1, 1);

            return uaBuilder.ToString();
        }

        #endregion 方法
    }
}