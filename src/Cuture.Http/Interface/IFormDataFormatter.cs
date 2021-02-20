namespace Cuture.Http
{
    /// <summary>
    /// 表单数据格式化器
    /// </summary>
    public interface IFormDataFormatter
    {
        #region Public 方法

        /// <summary>
        /// 格式化到表单字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Format(object obj);

        /// <summary>
        /// 格式化到已转义的表单字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string FormatToEncoded(object obj);

        #endregion Public 方法
    }
}