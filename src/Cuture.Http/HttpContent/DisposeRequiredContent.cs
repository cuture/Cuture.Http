#pragma warning disable IDE0130

using System.Diagnostics;

namespace Cuture.Http;

/// <summary>
/// 需要释放资源的 <see cref="HttpContent"/>
/// </summary>
/// <inheritdoc cref="DisposeRequiredContent"/>
public abstract class DisposeRequiredContent(IDisposable disposable) : HttpContent
{
    #region Private 析构函数

    /// <summary>
    ///
    /// </summary>
    ~DisposeRequiredContent()
    {
        Debug.WriteLine("DisposeRequiredContent Finalizer has been called.");
        Dispose(false);
    }

    #endregion Private 析构函数

    #region Protected 方法

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        Debug.WriteLine("DisposeRequiredContent Dispose(bool) has been called.");
        base.Dispose(disposing);
        disposable.Dispose();
    }

    #endregion Protected 方法
}
