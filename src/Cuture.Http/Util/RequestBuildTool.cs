#if NET
using System;
using System.Runtime.CompilerServices;
using System.Text;

using static Cuture.Http.Internal.HttpRawDataReadTool;

namespace Cuture.Http
{
    /// <summary>
    /// 请求构建工具
    /// </summary>
    public static class RequestBuildTool
    {
        /// <summary>
        /// 从请求的原始数据的base64字符串构建请求
        /// </summary>
        /// <param name="rawBase64String"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest FromRaw(string rawBase64String) => FromRaw(Convert.FromBase64String(rawBase64String));

        /// <summary>
        /// 从请求的原始数据构建请求
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest FromRaw(byte[] data) => FromRaw(data.AsSpan());

        /// <summary>
        /// 从请求的原始数据构建请求
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IHttpTurboRequest FromRaw(ReadOnlySpan<byte> data)
        {
            if (data.Length > 5 * 1024 * 1024)
            {
                throw new ArgumentOutOfRangeException($"the data (length:{data.Length}) is too large, This is not the recommend way to use it.");
            }

            ReadHttpRequestLine(ref data, out var methodSpan, out var urlSpan, out _);

            var encoding = Encoding.UTF8;

            var url = encoding.GetString(urlSpan);
            var request = url.ToHttpRequest()
                             .UseVerb(encoding.GetString(methodSpan));

            var (contentLength, contentType) = request.LoadHeaders(ref data, encoding);

            if (data.Length > 0)
            {
                request.WithContent(data, contentType, contentLength);
            }

            return request;
        }

        /// <summary>
        /// 读取请求行
        /// </summary>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="version"></param>
        internal static void ReadHttpRequestLine(ref ReadOnlySpan<byte> data, out ReadOnlySpan<byte> method, out ReadOnlySpan<byte> url, out ReadOnlySpan<byte> version)
        {
            method = ReadSegmentData(ref data, SpaceSeparator);
            if (method.IsEmpty)
            {
                throw new ArgumentException("not found “method” in data. Please check the raw data.");
            }

            url = ReadSegmentData(ref data, SpaceSeparator);
            if (url.IsEmpty)
            {
                throw new ArgumentException("not found “url” in data. Please check the raw data.");
            }

            version = ReadSegmentData(ref data, NewLineSeparatorSpan);
            if (version.IsEmpty)
            {
                throw new ArgumentException("not found “version” in data. Please check the raw data.");
            }
        }

        /// <summary>
        /// 加载header到指定<see cref="System.Net.Http.Headers.HttpHeaders"/>，并返回contentLength和contentType
        /// <para/>
        /// 当headers为空时不进行加载，但仍然会读取内容
        /// </summary>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        internal static (int contentLength, string contentType) LoadHeaders(ref ReadOnlySpan<byte> data, System.Net.Http.Headers.HttpHeaders? headers, Encoding? encoding)
        {
            var newLineSeparator = NewLineSeparatorSpan;
            var contentLength = -1;
            var contentType = string.Empty;
            encoding ??= Encoding.UTF8;

            while (data.Length > 0)
            {
                var headerSpan = ReadSegmentData(ref data, newLineSeparator);

                //连续两次换行 - Header结束
                if (headerSpan.Length == 0)
                {
                    break;
                }

                var keySpan = ReadSegmentData(ref headerSpan, ColonSeparator);

                var key = encoding.GetString(keySpan);

                switch (key)
                {
                    case HttpHeaderDefinitions.Host:  //忽略Host
                        continue;
                    case HttpHeaderDefinitions.ContentType:
                        contentType = GetHeaderValue(encoding, headerSpan);
                        continue;
                    case HttpHeaderDefinitions.ContentLength:
                        contentLength = int.Parse(GetHeaderValue(encoding, headerSpan));
                        continue;
                }
                headers?.TryAddWithoutValidation(key, GetHeaderValue(encoding, headerSpan));
            }

            return (contentLength, contentType);
        }
    }
}
#endif