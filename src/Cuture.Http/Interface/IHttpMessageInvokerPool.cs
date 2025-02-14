#pragma warning disable IDE0130

namespace Cuture.Http;

/// <summary>
/// <see cref="HttpMessageInvoker"/> 池
/// </summary>
public interface IHttpMessageInvokerPool : IDisposable
{
    #region 方法

    /// <summary>
    /// 获取 <paramref name="request"/> 对应使用的 <see cref="HttpMessageInvoker"/>
    /// </summary>
    /// <param name="request">请求</param>
    /// <returns></returns>
    IOwner<HttpMessageInvoker> Rent(IHttpRequest request);

    #endregion 方法
}
