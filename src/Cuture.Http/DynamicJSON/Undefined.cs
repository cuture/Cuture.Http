using System;
using System.Dynamic;

namespace Cuture.Http.DynamicJSON;

/// <summary>
/// 用于确认是否是未定义的对象字段，类比 Javascript 关键字 undefined
/// </summary>
internal sealed class Undefined : DynamicObject
{
    #region Private 字段

    private readonly string _name;

    #endregion Private 字段

    #region Internal 字段

    internal static readonly Undefined Instance = new();

    #endregion Internal 字段

    #region Private 构造函数

    /// <inheritdoc cref="Undefined"/>
    public Undefined(string name)
    {
        _name = name;
    }

    private Undefined()
    {
        _name = null!;
    }

    #endregion Private 构造函数

    #region Public 方法

    /// <summary>
    /// 是否为 undefined （排除为 null 的情况）
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsUndefined(object? value)
    {
        return value is Undefined;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(Undefined a, object b)
    {
        return b is not null && b is not Undefined;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(Undefined a, object b)
    {
        return b is null || b is Undefined;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is null || obj is Undefined;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => -1;

    #region Override

    public override string ToString() => "undefined";

    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result) => throw GetInvalidOperationException();

    public override bool TryConvert(ConvertBinder binder, out object? result) => throw GetInvalidOperationException();

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result) => throw GetInvalidOperationException();

    public override bool TryGetMember(GetMemberBinder binder, out object? result) => throw GetInvalidOperationException();

    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result) => throw GetInvalidOperationException();

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result) => throw GetInvalidOperationException();

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) => throw GetInvalidOperationException();

    public override bool TrySetMember(SetMemberBinder binder, object? value) => throw GetInvalidOperationException();

    public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result) => throw GetInvalidOperationException();

    private Exception GetInvalidOperationException() => new InvalidOperationException($"\"{_name}\" is undefined.");

    #endregion Override

    #endregion Public 方法
}
