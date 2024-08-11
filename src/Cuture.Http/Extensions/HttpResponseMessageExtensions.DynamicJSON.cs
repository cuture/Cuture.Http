using System.Runtime.CompilerServices;

using Cuture.Http.DynamicJSON;

namespace Cuture.Http;

public static partial class HttpResponseMessageExtensions
{
    #region Task<HttpResponseMessage>

    #region json as DynamicJson

    /// <summary>
    /// 以 json 接收返回数据，并解析为可动态访问的 dynamic 对象
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<dynamic?> ReceiveAsDynamicJsonAsync(this Task<HttpRequestExecuteState> requestTask, CancellationToken cancellationToken = default)
    {
        using var executeState = await requestTask.ConfigureAwait(false);
        return await executeState.HttpResponseMessage.ReceiveAsDynamicJsonAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 尝试以 json 接收返回数据，并解析为可动态访问的 dynamic 对象
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="textRequired">需要原始文本</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TextHttpOperationResult<dynamic>> TryReceiveAsDynamicJsonAsync(this Task<HttpRequestExecuteState> requestTask, bool textRequired = false, CancellationToken cancellationToken = default)
    {
        var result = new TextHttpOperationResult<dynamic>();
        try
        {
            using var executeState = await requestTask.ConfigureAwait(false);

            result.ResponseMessage = executeState.HttpResponseMessage;

            if (!textRequired)
            {
                result.Data = await result.ResponseMessage.ReceiveAsDynamicJsonAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var json = await result.ResponseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                result.Text = json;

                result.Data = JSON.parse(json);
            }
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        return result;
    }

    #endregion json as DynamicJson

    #endregion Task<HttpResponseMessage>

    #region ReceiveData

    /// <summary>
    /// 获取请求返回值并转换为可动态访问的 dynamic 对象
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<dynamic?> ReceiveAsDynamicJsonAsync(this HttpResponseMessage responseMessage,
                                                                 CancellationToken cancellationToken = default)
    {
        var json = await responseMessage.ReceiveAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JSON.parse(json);
    }

    #endregion ReceiveData
}
