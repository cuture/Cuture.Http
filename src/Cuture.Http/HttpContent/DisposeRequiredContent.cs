using System;
using System.Diagnostics;
using System.Net.Http;

namespace Cuture.Http;

/// <summary>
/// 需要释放资源的 <see cref="HttpContent"/>
/// </summary>
public abstract class DisposeRequiredContent : HttpContent
{
    #region Private 字段

    private readonly IDisposable _disposable;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="DisposeRequiredContent"/>
    public DisposeRequiredContent(IDisposable disposable)
    {
        _disposable = disposable;
    }

    #endregion Public 构造函数

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
        _disposable.Dispose();
    }

    #endregion Protected 方法
}
