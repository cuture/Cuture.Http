#if NET
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cuture.Http.Internal
{
    /// <summary>
    /// Http原始数据读取工具
    /// </summary>
    internal static class HttpRawDataReadTool
    {
        /// <summary>
        /// 空格分隔符
        /// </summary>
        public const byte SpaceSeparator = (byte)' ';

        /// <summary>
        /// 冒号分隔符
        /// </summary>
        public const byte ColonSeparator = (byte)':';

        /// <summary>
        /// 新行分隔符
        /// </summary>
        internal static readonly byte[] NewLineSeparator = new[] { (byte)'\r', (byte)'\n' };

        /// <summary>
        /// 新行分隔符
        /// </summary>
        internal static ReadOnlySpan<byte> NewLineSeparatorSpan => NewLineSeparator.AsSpan();

        /// <summary>
        /// Header结束分隔符
        /// </summary>
        internal static readonly byte[] HeaderEndSeparator = new[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' };

        /// <summary>
        /// Header结束分隔符
        /// </summary>
        internal static ReadOnlySpan<byte> HeaderEndSeparatorSpan => HeaderEndSeparator.AsSpan();

        /// <summary>
        /// 读取Header的值（自动忽略头部的空格）
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetHeaderValue(Encoding encoding, in ReadOnlySpan<byte> data) => GetStringWithTrimStartOnce(encoding, data, (byte)' ');

        /// <summary>
        /// 获取字符串，并自动修剪一次指定的头部字符
        /// </summary>
        /// <param name="encoding">字符编码</param>
        /// <param name="data">数据</param>
        /// <param name="trimValue">修剪的值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetStringWithTrimStartOnce(Encoding encoding, in ReadOnlySpan<byte> data, byte trimValue)
        {
            return data[0] == trimValue
                        ? encoding.GetString(data[1..])
                        : encoding.GetString(data);
        }

        /// <summary>
        /// 读取并将数据调整为读取后的剩余数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">读取的数据长度</param>
        /// <param name="lengthFix">修正处理长度的值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> ReadAndReduceData(ref ReadOnlySpan<byte> data, int length, int lengthFix)
        {
            var result = data.Slice(0, length);
            data = data[(length + lengthFix)..];

            return result;
        }

        /// <summary>
        /// 根据指定终止值读取片段数据，并将数据调整为读取后的剩余数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="terminal">终止值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> ReadSegmentData(ref ReadOnlySpan<byte> data, byte terminal)
        {
            var index = data.IndexOf(terminal);
            if (index < 0)
            {
                return ReadOnlySpan<byte>.Empty;
            }
            return ReadAndReduceData(ref data, index, 1);
        }

        /// <summary>
        /// 根据指定终止值读取片段数据，并将数据调整为读取后的剩余数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="terminal">终止值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> ReadSegmentData(ref ReadOnlySpan<byte> data, in ReadOnlySpan<byte> terminal)
        {
            var index = data.IndexOf(terminal);
            if (index < 0)
            {
                return ReadOnlySpan<byte>.Empty;
            }
            return ReadAndReduceData(ref data, index, terminal.Length);
        }
    }
}
#endif