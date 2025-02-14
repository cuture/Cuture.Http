using System.Dynamic;

namespace Cuture.Http.DynamicJSON;

/// <summary>
/// 用于确认是否是未定义的对象字段，类比 Javascript 关键字 undefined
/// </summary>
public sealed class Undefined : DynamicObject
{
    #region Undefined访问传播

    /// <summary>
    /// <see cref="Undefined"/> 访问传播 (可以访问 <see cref="Undefined"/> 的成员并返回为 <see cref="Undefined"/>)
    /// </summary>
    [ThreadStatic]
    private static bool s_undefinedPropagation;

    #endregion Undefined访问传播

    #region Private 字段

    private readonly string _name;

    #endregion Private 字段

    #region Public 字段

    /// <summary>
    /// Undefined 实例
    /// </summary>
    public static readonly Undefined Instance = new();

    #endregion Public 字段

    #region Private 构造函数

    /// <inheritdoc cref="Undefined"/>
    internal Undefined(string name)
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
    /// 是否为 undefined （排除为 null 的情况）<br/>
    /// 委托 <paramref name="proprytyAccessDelegate"/> 中可以访问 <see cref="Undefined"/> 的成员并返回为 <see cref="Undefined"/><br/>
    /// eg:<br/>
    /// 检查对象 user - {"name":"name","age":1} 是否定义了 box1.box2.box3 <br/>
    /// IsUndefined(() => user.box1.box2.box3)
    /// </summary>
    /// <param name="proprytyAccessDelegate">属性访问委托</param>
    /// <returns></returns>
    public static bool IsUndefined(in Func<object?> proprytyAccessDelegate)
    {
        try
        {
            s_undefinedPropagation = true;

            var value = proprytyAccessDelegate();

            return IsUndefined(value);
        }
        finally
        {
            s_undefinedPropagation = false;
        }
    }

    /// <summary>
    /// 是否为 undefined （排除为 null 的情况）
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsUndefined(in object? value) => value is Undefined;

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

    /// <inheritdoc/>
    public override string ToString() => "undefined";

    /// <inheritdoc/>
    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result) => throw GetInvalidOperationException();

    /// <inheritdoc/>
    public override bool TryConvert(ConvertBinder binder, out object? result) => throw GetInvalidOperationException();

    /// <inheritdoc/>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result) => throw GetInvalidOperationException();

    /// <inheritdoc/>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (s_undefinedPropagation)
        {
            result = this;
            return true;
        }
        throw GetInvalidOperationException();
    }

    /// <inheritdoc/>
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result) => throw GetInvalidOperationException();

    /// <inheritdoc/>
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        if ((args is null || args.Length == 0)
            && string.Equals(nameof(IsUndefined), binder.Name, StringComparison.OrdinalIgnoreCase))
        {
            result = JsonNodeUtil.ObjectTrue;
            return true;
        }
        throw GetInvalidOperationException();
    }

    /// <inheritdoc/>
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) => throw GetInvalidOperationException();

    /// <inheritdoc/>
    public override bool TrySetMember(SetMemberBinder binder, object? value) => throw GetInvalidOperationException();

    /// <inheritdoc/>
    public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result)
    {
        return binder.Operation switch
        {
            System.Linq.Expressions.ExpressionType.IsTrue => Result(false, out result),
            System.Linq.Expressions.ExpressionType.IsFalse => Result(true, out result),
            System.Linq.Expressions.ExpressionType.Not => Result(true, out result),
            _ => throw GetInvalidOperationException(),
        };

        static bool Result(object? resultValue, out object? result)
        {
            result = resultValue;
            return true;
        }
    }

    private InvalidOperationException GetInvalidOperationException() => new($"\"{_name}\" is undefined.");

    #endregion Override

    #endregion Public 方法
}
