using System.Threading;

namespace System
{
    //TODO .net6 使用 Random.Shared
    internal static class SharedRandom
    {
        #region Private 字段

        private static readonly ThreadLocal<Random> s_random = new(() => new(), false);

        #endregion Private 字段

        #region Public 属性

        public static Random Shared => s_random.Value;

        #endregion Public 属性
    }
}