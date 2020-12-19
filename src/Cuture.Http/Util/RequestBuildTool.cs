using System;
using System.Text;

namespace Cuture.Http
{
#if NET

    /// <summary>
    /// 请求构建工具
    /// </summary>
    public static class RequestBuildTool
    {
        /// <summary>
        /// 空格分隔符
        /// </summary>
        private const byte SpaceSplitChar = (byte)' ';

        /// <summary>
        /// 冒号分隔符
        /// </summary>
        private const byte ColonSplitChar = (byte)':';

        /// <summary>
        /// 新行分隔符
        /// </summary>
        private static readonly byte[] NewLineSplitChars = new[] { (byte)'\r', (byte)'\n' };

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

            var methodSpan = ReadSegmentData(ref data, SpaceSplitChar);

            if (methodSpan.IsEmpty)
            {
                throw new ArgumentException("not found “method” in data. Please check the raw data.");
            }

            var urlSpan = ReadSegmentData(ref data, SpaceSplitChar);

            if (urlSpan.IsEmpty)
            {
                throw new ArgumentException("not found “url” in data.Please check the raw data.");
            }

            var newLineSplitChars = NewLineSplitChars.AsSpan();

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

                var keySpan = ReadSegmentData(ref headerSpan, ColonSplitChar);

                var key = encoding.GetString(keySpan);

                switch (key)
                {
                    case HttpHeaders.Host:  //忽略Host
                        continue;
                    case HttpHeaders.ContentType:
                        contentType = GetHeaderValue(encoding, headerSpan);
                        continue;
                    case HttpHeaders.ContentLength:
                        contentLength = int.Parse(GetHeaderValue(encoding, headerSpan));
                        continue;
                }

                request.AddHeader(key, GetHeaderValue(encoding, headerSpan));
            }

            if (data.Length > 0)
            {
                request.Content = new TypedByteArrayContent(contentLength > 0 ? data.Slice(0, contentLength).ToArray() : data.ToArray(),
                                                            contentType);
            }

            return request;
        }

        #region Private

        private static string GetHeaderValue(Encoding encoding, in ReadOnlySpan<byte> header)
        {
            return header[0] == ' '
                        ? encoding.GetString(header[1..])
                        : encoding.GetString(header);
        }

        /// <summary>
        /// 读取片段数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static ReadOnlySpan<byte> ReadSegmentData(ref ReadOnlySpan<byte> data, byte value)
        {
            var index = data.IndexOf(value);
            if (index < 0)
            {
                return ReadOnlySpan<byte>.Empty;
            }
            return ReadAndReduceData(ref data, index, 1);
        }

        /// <summary>
        /// 读取片段数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static ReadOnlySpan<byte> ReadSegmentData(ref ReadOnlySpan<byte> data, in ReadOnlySpan<byte> value)
        {
            var index = data.IndexOf(value);
            if (index < 0)
            {
                return ReadOnlySpan<byte>.Empty;
            }
            return ReadAndReduceData(ref data, index, value.Length);
        }

        /// <summary>
        /// 读取并减少数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static ReadOnlySpan<byte> ReadAndReduceData(ref ReadOnlySpan<byte> data, int index, int indexFix)
        {
            var result = data.Slice(0, index);
            data = data[(index + indexFix)..];

            return result;
        }

        #endregion Private
    }

#endif
}