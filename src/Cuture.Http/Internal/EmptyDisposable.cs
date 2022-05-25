using System.Runtime.CompilerServices;

namespace System;

internal sealed class EmptyDisposable : IDisposable
{
    #region Public 属性

    public static EmptyDisposable Instance { get; } = new();

    #endregion Public 属性

    #region Public 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
    }

    #endregion Public 方法
}
