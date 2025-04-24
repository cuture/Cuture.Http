#pragma warning disable IDE0130

namespace Cuture.Http.Internal;

internal sealed class OrdinalReadOnlyMemoryCharComparer : IEqualityComparer<ReadOnlyMemory<char>>
{
    #region Public 属性

    public static OrdinalReadOnlyMemoryCharComparer Shared { get; } = new();

    #endregion Public 属性

    #region Public 方法

    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y) => x.Span.Equals(y.Span, StringComparison.Ordinal);

    public int GetHashCode(ReadOnlyMemory<char> obj) => string.GetHashCode(obj.Span, StringComparison.Ordinal);

    #endregion Public 方法
}
