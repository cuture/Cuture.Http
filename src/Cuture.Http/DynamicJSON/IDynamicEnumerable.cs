namespace Cuture.Http.DynamicJSON;

/// <summary>
/// 以 <see cref="IEnumerable{T}"/> 访问数组对象元素
/// <para/>T 为 dynamic?
/// </summary>
public interface IDynamicEnumerable
{
    #region Public 方法

    /// <summary>
    /// 转换为 <see cref="IEnumerable{T}"/>，T 为 dynamic?
    /// </summary>
    /// <returns></returns>
    IEnumerable<dynamic?> AsEnumerable();

    #endregion Public 方法
}

/// <summary>
/// 以 <see cref="IEnumerable{T}"/>访问对象元素
/// <para/>T 为 <see cref="KeyValuePair{TKey, TValue}"/>，TKey 为 string，TValue 为 dynamic?
/// </summary>
public interface IDynamicKeyValueEnumerable
{
    #region Public 方法

    /// <summary>
    /// 转换为<see cref="IEnumerable{T}"/>
    /// <para/>T 为 <see cref="KeyValuePair{TKey, TValue}"/>，TKey 为 string，TValue 为 dynamic?
    /// </summary>
    /// <returns></returns>
    IEnumerable<KeyValuePair<string, dynamic?>> AsEnumerable();

    #endregion Public 方法
}
