namespace Cuture.Http;

/// <summary>
/// 所有者
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IOwner<out T> : IDisposable
    where T : class
{
    #region Public 属性

    /// <summary>
    /// 值
    /// </summary>
    public T Value { get; }

    #endregion Public 属性
}
