using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    //此文件主要包含 内部使用 相关的拓展方法

    public static partial class IHttpRequestBuildExtensions
    {
        #region Internal 方法

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IJsonSerializer GetJsonSerializerOrDefault(this IHttpRequest request)
        {
            return request.IsSetOptions && request.ExecutionOptions.JsonSerializer != null
                                    ? request.ExecutionOptions.JsonSerializer
                                    : HttpRequestGlobalOptions.DefaultJsonSerializer;
        }

        #endregion Internal 方法
    }
}