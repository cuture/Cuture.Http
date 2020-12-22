#if NET
using System;
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
        public static IHttpTurboRequest FromRaw(string rawBase64String) => FromRaw(Convert.FromBase64String(rawBase64String));

        /// <summary>
        /// 从请求的原始数据构建请求
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

            var methodSpan = ReadSegmentData(ref data, SpaceSeparator);

            if (methodSpan.IsEmpty)
            {
                throw new ArgumentException("not found “method” in data. Please check the raw data.");
            }

            var urlSpan = ReadSegmentData(ref data, SpaceSeparator);

            if (urlSpan.IsEmpty)
            {
                throw new ArgumentException("not found “url” in data.Please check the raw data.");
            }

            var newLineSplitChars = NewLineSeparator.AsSpan();

            //暂时不处理http版本
            var httpVersionSpan = ReadSegmentData(ref data, newLineSplitChars);

            var encoding = Encoding.UTF8;

            var url = encoding.GetString(urlSpan);
            var request = url.ToHttpRequest()
                             .UseVerb(encoding.GetString(methodSpan));

            int contentLength = -1;
            var contentType = string.Empty;

            while (data.Length > 0)
            {
                var headerSpan = ReadSegmentData(ref data, newLineSplitChars);

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
                request.Headers.TryAddWithoutValidation(key, GetHeaderValue(encoding, headerSpan));
            }

            if (data.Length > 0)
            {
                request.WithContent(data, contentType, contentLength);
            }

            return request;
        }
    }
}
#endif